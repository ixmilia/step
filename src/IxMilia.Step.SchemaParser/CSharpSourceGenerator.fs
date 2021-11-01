namespace IxMilia.Step.SchemaParser

open System

module CSharpSourceGenerator =
    let private indentation = "    "
    let toLines (text: string) = text.Split('\n')
    let indentLine (line: string) = if line.Length > 0 then indentation + line else line
    let indentLines lines = lines |> Seq.map indentLine
    let joinWith (sep: string) (parts: string seq) = String.Join(sep, parts)
    let joinLines = joinWith "\n"
    let getIdentifierName (nameFromSchema: string) =
        nameFromSchema.Split('_')
        |> Array.map (fun p -> p.Substring(0, 1).ToUpperInvariant() + p.Substring(1))
        |> Array.fold (+) ""
    let getParameterName (nameFromSchema: string) =
        let identifierName = getIdentifierName nameFromSchema
        identifierName.Substring(0, 1).ToLowerInvariant() + identifierName.Substring(1)
    let getFieldName (nameFromSchema: string) = "_" + getParameterName nameFromSchema
    let getIdentifierNameWithPrefix (nameFromSchema: string) (typeNamePrefix: string) =
        typeNamePrefix + getIdentifierName nameFromSchema
    let private getConstructedTypeName (constructedType: ConstructedType) (typeNamePrefix: string) =
        match constructedType with
        | EnumerationType values -> ""
        | SelectType values -> ""
    let private getSimpleTypeName (simpleType: SimpleType) (_typeNamePrefix: string) =
        match simpleType with
        | BinaryType (_width, _isFixed) -> "byte[]"
        | BooleanType -> "bool"
        | IntegerType -> "long"
        | LogicalType -> "bool"
        | NumberType -> "double"
        | RealType _precision -> "double"
        | StringType (_width, _isFixed) -> "string"
    let private generateSimpleType (simpleType: SimpleType): string option =
        let hasCustomTypeDefinintion =
            match simpleType with
            | _ -> false
        if hasCustomTypeDefinintion then
            match simpleType with
            | _ -> failwith "not supported"
        else None
    let rec private isBuiltInType (baseType: BaseType) (namedTypeOverrides: Map<string, BaseType>) =
        match baseType with
        | SimpleType _ -> true
        | NamedType n ->
            match Map.tryFind n namedTypeOverrides with
            | Some _ -> true
            | None -> false
        | AggregationType a ->
            match a with
            | ListType (typ, lowerBound, upperBound, _isUnique) ->
                match (lowerBound, upperBound) with
                | (LiteralValue(IntegerLiteral(lower)), Some(LiteralValue(IntegerLiteral(upper)))) when lower >= 0L && upper = 3L ->
                    match typ with
                    | SimpleType _
                    | NamedType "length_measure" -> true
                    | _ -> false
                | _ -> false
            | _ -> false
        | _ -> false
    let getBaseTypeName (baseType: BaseType) (typeNamePrefix: string) (namedTypeOverrides: Map<string, BaseType>) =
        let rec getBaseTypeName' (baseType: BaseType) (typeNamePrefix: string) (namedTypeOverrides: Map<string, BaseType>) (normalizeName: bool) =
            match baseType with
            | ConstructedType c -> "TODO"
            | AggregationType a ->
                match a with
                | ListType (typ, lowerBound, upperBound, _isUnique) ->
                    match (lowerBound, upperBound) with
                    | (LiteralValue(IntegerLiteral(lower)), Some(LiteralValue(IntegerLiteral(upper)))) when lower >= 0L && upper = 3L ->
                        match typ with
                        | SimpleType(RealType(_))
                        | NamedType "length_measure" ->
                            getIdentifierNameWithPrefix "Vector3D" typeNamePrefix
                        | _ -> "TODO"
                    | _ -> "TODO"
                | _ -> "TODO"
            | SimpleType s -> getSimpleTypeName s typeNamePrefix
            | NamedType n ->
                match Map.tryFind n namedTypeOverrides with
                | Some namedTypeOverride -> getBaseTypeName' namedTypeOverride "" namedTypeOverrides false
                | None ->
                    if normalizeName then getIdentifierNameWithPrefix n typeNamePrefix
                    else n
        getBaseTypeName' baseType typeNamePrefix namedTypeOverrides true
    let getNamedTypeOverride (typ: BaseType): BaseType option =
        match typ with
        | SimpleType s -> Some(NamedType(getSimpleTypeName s ""))
        | _ -> None
    let getNamedTypeOverrideMap (types: SchemaType list): Map<string, BaseType> =
        types
        |> List.map (fun t -> (t, getNamedTypeOverride t.Type))
        |> List.filter (snd >> Option.isSome)
        |> List.map (fun (a, b) -> (a.Name, Option.get b))
        |> Map.ofList
    let getExpressionCode (expression: Expression): string option =
        let rec getValidationStatementPredicate' (expression: Expression): string option =
            let testAndCombine (a: string option) (b: string option) (template: string): string option =
                match (a, b) with
                | (Some a, Some b) -> Some(String.Format(template, a, b))
                | _ -> None
            match expression with
            | LiteralValue l -> Some(l.ToString())
            | ReferencedAttributeExpression r ->
                let qualificationText =
                    match r.Qualification with
                    | Some(ReferencedAttributeQualificationWithAttribute r2)
                    | Some(ReferencedAttributeQualificationWithGroup r2) ->
                        match getValidationStatementPredicate' (ReferencedAttributeExpression r2) with
                        | Some r2 -> Some("." + r2.ToString())
                        | None -> None
                    | None -> Some ""
                match qualificationText with
                | Some qt ->
                    let attributeName =
                        match r.Name.ToUpperInvariant() with
                        | "SELF" -> "this"
                        | _ -> r.Name |> getIdentifierName
                    Some(attributeName + qt)
                | None -> None
            | Negate n -> 
                match getValidationStatementPredicate' n with
                | Some p -> Some("-" + p)
                | None -> None
            | Add (a, b) -> testAndCombine (getValidationStatementPredicate' a) (getValidationStatementPredicate' b) "({0} + {1})"
            | Subtract (a, b) -> testAndCombine (getValidationStatementPredicate' a) (getValidationStatementPredicate' b) "({0} - {1})"
            | Multiply (a, b) -> testAndCombine (getValidationStatementPredicate' a) (getValidationStatementPredicate' b) "({0} * {1})"
            | Divide (a, b) -> testAndCombine (getValidationStatementPredicate' a) (getValidationStatementPredicate' b) "({0} / {1})"
            | Modulus (a, b) -> testAndCombine (getValidationStatementPredicate' a) (getValidationStatementPredicate' b) "({0} % {1})"
            | Exponent (a, b) -> testAndCombine (getValidationStatementPredicate' a) (getValidationStatementPredicate' b) "Math.Pow({0}, {1})"
            | Greater (a, b) -> testAndCombine (getValidationStatementPredicate' a) (getValidationStatementPredicate' b) "({0} > {1})"
            | GreaterEquals (a, b) -> testAndCombine (getValidationStatementPredicate' a) (getValidationStatementPredicate' b) "({0} >= {1})"
            | Less (a, b) -> testAndCombine (getValidationStatementPredicate' a) (getValidationStatementPredicate' b) "({0} < {1})"
            | LessEquals (a, b) -> testAndCombine (getValidationStatementPredicate' a) (getValidationStatementPredicate' b) "({0} <= {1})"
            | Equals (a, b) -> testAndCombine (getValidationStatementPredicate' a) (getValidationStatementPredicate' b) "({0} == {1})"
            | NotEquals (a, b) -> testAndCombine (getValidationStatementPredicate' a) (getValidationStatementPredicate' b) "({0} != {1})"
            | Assignable (a, b, _isStrict) -> testAndCombine (getValidationStatementPredicate' a) (getValidationStatementPredicate' b) "({0} is {1})"
            | NotAssignable (a, b) -> testAndCombine (getValidationStatementPredicate' a) (getValidationStatementPredicate' b) "!({0} is {1})"
            | FunctionCallExpression f ->
                // check for well-known function names
                let clrFunction =
                    match f.Name.ToLowerInvariant() with
                    | "sqrt" -> Some "Math.Sqrt"
                    // the following are defined later in the schema, but for now we'll hard-code it
                    | "dimension_of" -> Some(getIdentifierName f.Name)
                    // not supported
                    | _ -> None
                match clrFunction with
                | Some clrFunction ->
                    let argumentSet =
                        f.Arguments
                        |> List.map getValidationStatementPredicate'
                        |> List.fold (fun state t ->
                            match (state, t) with
                            | (Some collection, Some value) -> Some(List.append collection [value])
                            | _ -> None) (Some [])
                    match argumentSet with
                    | Some arguments -> Some(sprintf "%s(%s)" clrFunction (arguments |> joinWith ", "))
                    | None -> None
                | None -> None
            | QueryExpression _ -> None // NYI
            | ArrayExpression _ -> None // NYI
            | SubcomponentQualifiedExpression (_, _, _) -> failwith "subcomponent NYI"
            | In (_, _) -> failwith "in NYI"
            | Or (a, b) -> testAndCombine (getValidationStatementPredicate' a) (getValidationStatementPredicate' b) "({0} || {1})"
            | Xor (a, b) -> testAndCombine (getValidationStatementPredicate' a) (getValidationStatementPredicate' b) "({0} ^ {1})"
            | And (a, b) -> testAndCombine (getValidationStatementPredicate' a) (getValidationStatementPredicate' b) "({0} && {1})"
            | Not n ->
                match getValidationStatementPredicate' n with
                | Some n -> Some(sprintf "!(%s)" n)
                | None -> None
        getValidationStatementPredicate' expression
    let private getExplicitAttributeDeclaration (attr: ExplicitAttribute) (typeNamePrefix: string) (namedTypeOverrides: Map<string, BaseType>) =
        let attributeType = getBaseTypeName attr.Type.Type typeNamePrefix namedTypeOverrides
        let attributeName = getIdentifierName attr.AttributeDeclaration.Name
        let fieldName = getFieldName attr.AttributeDeclaration.Name
        seq {
            yield sprintf "internal %s %s;" attributeType fieldName
            yield sprintf "public %s %s" attributeType attributeName
            yield "{"
            yield sprintf "get => %s;" fieldName |> indentLine
            yield "set" |> indentLine
            yield "{" |> indentLine
            yield sprintf "%s = value;" fieldName |> indentLine |> indentLine
            yield "ValidateDomainRules();" |> indentLine |> indentLine
            yield "}" |> indentLine
            yield "}"
        } |> joinLines
    let private getDerivedAttributeDeclaration (attr: DerivedAttribute) (typeNamePrefix: string) (namedTypeOverrides: Map<string, BaseType>) =
        let attributeType = getBaseTypeName attr.Type.Type typeNamePrefix namedTypeOverrides
        let attributeName = getIdentifierName attr.AttributeDeclaration.Name
        let attributeExpression = getExpressionCode attr.Expression
        match attributeExpression with
        | Some ae -> sprintf "public %s %s => %s;" attributeType attributeName ae
        | None -> ""
    let private getValidationStatementBody (domainRule: DomainRule) =
        let predicate =
            match getExpressionCode domainRule.Expression with
            | Some p -> p
            | None -> "true /* TODO: not all validation predicates are supported */"
        let domainRuleText = domainRule.ToString()
        seq {
            yield sprintf "if (!%s)" predicate
            yield "{"
            yield sprintf "throw new StepValidationException(\"The validation rule '%s' was not satisfied\");" (domainRuleText.Replace("\"", "\\\"")) |> indentLine
            yield "}"
        } |> joinLines
    let private getDomainRuleValidationFunction (domainRules: DomainRule list) =
        let allDomainRules =
            domainRules
            |> List.map getValidationStatementBody
            |> List.map toLines
            |> Seq.concat
        seq {
            yield "internal override void ValidateDomainRules()"
            yield "{"
            yield "base.ValidateDomainRules();" |> indentLine
            yield! allDomainRules |> indentLines
            yield "}"
        } |> joinLines
    let rec private getEntityAndParentAttributes (schema: Schema) (entity: Entity) =
        let parentAttributes =
            match entity.SubTypes with
            | [] -> []
            | subTypeName::[] ->
                let parentEntity = schema.Entities |> List.find (fun e -> e.Name = subTypeName)
                let (p, pp) = getEntityAndParentAttributes schema parentEntity
                List.append p pp
            | _ -> failwith "multiple inheritance nyi"
        (parentAttributes, entity.Attributes)
    let private getReferencedItems (schema: Schema) (entity: Entity) (defaultBaseClassName: string) (namedTypeOverrides: Map<string, BaseType>) =
        seq {
            yield sprintf "internal override IEnumerable<%s> GetReferencedItems()" defaultBaseClassName
            yield "{"
            yield! getEntityAndParentAttributes schema entity
                   |> snd
                   |> Seq.filter (fun attribute -> not (isBuiltInType attribute.Type.Type namedTypeOverrides))
                   |> Seq.map (fun attribute -> sprintf "yield return %s;" (getIdentifierName attribute.AttributeDeclaration.Name))
                   |> indentLines
            yield indentLine "yield break;"
            yield "}"
        } |> joinLines
    let private getParametersFunction (schema: Schema) (entity: Entity) =
        seq {
            yield "internal override IEnumerable<StepSyntax> GetParameters(StepWriter writer)"
            yield "{"
            yield! seq {
                yield "foreach (var parameter in base.GetParameters(writer))"
                yield "{"
                yield indentLine "yield return parameter;"
                yield "}"
            } |> indentLines
            yield ""
            yield! getEntityAndParentAttributes schema entity
                   |> snd
                   |> Seq.map (fun attribute -> sprintf "yield return writer.GetItemSyntax(%s);" (getIdentifierName attribute.AttributeDeclaration.Name))
                   |> indentLines
            yield "}"
        } |> joinLines
    let private getCreationFromSyntaxFunction (schema: Schema) (entity: Entity) (typeNamePrefix: string) (namedTypeOverrides: Map<string, BaseType>) =
        let entityName = getIdentifierNameWithPrefix entity.Name typeNamePrefix
        let (parentEntityAttributes, thisEntityAttributes) = getEntityAndParentAttributes schema entity
        let allAttributeCount = parentEntityAttributes.Length + thisEntityAttributes.Length
        let getSettersFromAttributes (attributes: ExplicitAttribute list) (indexOffset: int) =
            attributes
            |> Seq.mapi (fun i e ->
                let fieldName = getFieldName e.AttributeDeclaration.Name
                let index = i + indexOffset
                let baseTypeName = getBaseTypeName e.Type.Type typeNamePrefix namedTypeOverrides
                if isBuiltInType e.Type.Type namedTypeOverrides then
                    sprintf "item.%s = syntaxList.Values[%d].Get%sValue();" fieldName index (getIdentifierName baseTypeName)
                else
                    sprintf "binder.BindValue(syntaxList.Values[%d], value => item.%s = value.AsType<%s>());" index fieldName baseTypeName)
        seq {
            yield sprintf "internal static new %s CreateFromSyntaxList(StepBinder binder, StepSyntaxList syntaxList)" entityName
            yield "{"
            yield! seq {
                yield sprintf "syntaxList.AssertListCount(%d);" allAttributeCount
                yield sprintf "var item = new %s();" entityName
                yield! getSettersFromAttributes parentEntityAttributes 0
                yield! getSettersFromAttributes thisEntityAttributes parentEntityAttributes.Length
                yield "return item;"
            } |> indentLines
            yield "}"
        } |> joinLines
    let getSchemaTypeName (schemaType: SchemaType) (typeNamePrefix: string) =
        match schemaType.Type with
        | SimpleType s -> getSimpleTypeName s typeNamePrefix
        | _ -> getIdentifierNameWithPrefix schemaType.Name typeNamePrefix
    let getSchemaTypeDefinition (schemaType: SchemaType) (typeNamePrefix: string): string option =
        let typeName = getSchemaTypeName schemaType typeNamePrefix
        match schemaType.Type with
        | ConstructedType c ->
            match c with
            | EnumerationType values ->
                let enumValues =
                    values
                    |> List.map getIdentifierName
                    |> List.map (fun v -> indentation + v + ",\n")
                    |> List.fold (+) ""
                Some $"public enum {typeName}\n{{\n{enumValues}}}\n"
            | SelectType types -> Some ""
        | AggregationType a -> Some ""
        | SimpleType s -> generateSimpleType s
        | NamedType n -> Some ""
    let getEntityDeclaration (entity: Entity) (typeNamePrefix: string) (defaultBaseClassName: string): string =
        let entityName = getIdentifierNameWithPrefix entity.Name typeNamePrefix
        let subTypeDefinitionText =
            match entity.SubTypes with
            | [] -> sprintf " : %s" defaultBaseClassName
            | subTypeName::[] -> sprintf " : %s" (getIdentifierNameWithPrefix subTypeName typeNamePrefix)
            | _ -> failwith "multiple inheritance NYI"
        sprintf "public class %s%s" entityName subTypeDefinitionText
    let getEntityDefinition (schema: Schema) (entity: Entity) (generatedNamespace: string) (usingNamespaces: string seq) (typeNamePrefix: string) (defaultBaseClassName: string) (namedTypeOverrides: Map<string, BaseType>): (string * string) =
        let entityDeclaration = getEntityDeclaration entity typeNamePrefix defaultBaseClassName
        let explicitAttributeLines =
            entity.Attributes
            |> List.map (fun a -> getExplicitAttributeDeclaration a typeNamePrefix namedTypeOverrides)
            |> joinWith "\n\n"
            |> toLines
            |> indentLines
        let derivedAttributeLines =
            entity.DerivedAttributes
            |> List.map (fun a -> getDerivedAttributeDeclaration a typeNamePrefix namedTypeOverrides)
            |> joinWith "\n\n"
            |> toLines
            |> indentLines
        let allAttributeLines = Seq.append explicitAttributeLines derivedAttributeLines
        let validationRuleFunction = getDomainRuleValidationFunction entity.DomainRules |> toLines |> indentLines |> joinLines
        let getReferencedItems = getReferencedItems schema entity defaultBaseClassName namedTypeOverrides |> toLines |> indentLines |> joinLines
        let getParametersFunction = getParametersFunction schema entity |> toLines |> indentLines |> joinLines
        let entityCreationFunction = getCreationFromSyntaxFunction schema entity typeNamePrefix namedTypeOverrides |> toLines |> indentLines |> joinLines
        let (parentEntityAttributes, thisEntityAttributes) = getEntityAndParentAttributes schema entity
        let getAttributesAsArgumentTexts (attributes: ExplicitAttribute list) =
            attributes
            |> List.map (fun a -> sprintf "%s %s" (getBaseTypeName a.Type.Type typeNamePrefix namedTypeOverrides) (a.AttributeDeclaration.Name |> getParameterName))
        let thisArgumentTexts = getAttributesAsArgumentTexts thisEntityAttributes
        let parentArgumentTexts = getAttributesAsArgumentTexts parentEntityAttributes
        let allArgumentsText = List.append parentArgumentTexts thisArgumentTexts |> joinWith ", "
        let constructorLines =
            let entityName = getIdentifierNameWithPrefix entity.Name typeNamePrefix
            seq {
                yield sprintf "internal %s()" entityName
                yield "{"
                yield "}"
                yield ""
                yield sprintf "public %s(%s)" entityName allArgumentsText
                yield "{"
                yield! Seq.append (parentEntityAttributes |> Seq.map (fun a -> a.AttributeDeclaration.Name))
                                  (thisEntityAttributes |> Seq.map (fun a -> a.AttributeDeclaration.Name))
                       |> Seq.map (fun name -> sprintf "%s = %s;" (getFieldName name) (getParameterName name))
                       |> indentLines
                yield "ValidateDomainRules();" |> indentLine
                yield "}"
            } |> indentLines
        let generatedCode =
            seq {
                yield! usingNamespaces |> Seq.map (fun ns -> sprintf "using %s;" ns)
                yield ""
                yield sprintf "namespace %s" generatedNamespace
                yield "{"
                yield! entityDeclaration |> toLines |> indentLines
                yield "{" |> indentLine
                yield sprintf "public override string ItemTypeString => \"%s\";" (entity.Name.ToUpperInvariant()) |> indentLine |> indentLine
                yield ""
                yield! allAttributeLines |> indentLines
                yield ""
                yield! constructorLines |> indentLines
                yield ""
                yield! validationRuleFunction |> toLines |> indentLines
                yield ""
                yield! getReferencedItems |> toLines |> indentLines
                yield ""
                yield! getParametersFunction |> toLines |> indentLines
                yield ""
                yield! entityCreationFunction |> toLines |> indentLines
                yield "}" |> indentLine
                yield "}"
                yield ""
            } |> joinLines
        let entityName = getIdentifierNameWithPrefix entity.Name typeNamePrefix
        (entityName, generatedCode)
    let getEntityDefinitions (schema: Schema) (generatedNamespace: string) (usingNamespaces: string seq) (typeNamePrefix: string) (defaultBaseClassName: string): (string * string) list =
        let namedTypeOverrides = getNamedTypeOverrideMap schema.Types
        schema.Entities
        |> List.map (fun e -> getEntityDefinition schema e generatedNamespace usingNamespaces typeNamePrefix defaultBaseClassName namedTypeOverrides)
    let getFromItemSyntaxFile (schema: Schema) (generatedNamespace: string) (typeNamePrefix: string) (defaultBaseClassName: string): (string * string) =
        let itemName = sprintf "%sBuilder" defaultBaseClassName
        let content =
            seq {
                yield "using IxMilia.Step.Syntax;"
                yield ""
                yield sprintf "namespace %s" generatedNamespace
                yield "{"
                yield! seq {
                    yield sprintf "internal static class %s" itemName
                    yield "{"
                    yield! seq {
                        yield sprintf "internal static %s FromTypedParameter(StepBinder binder, StepItemSyntax itemSyntax)" defaultBaseClassName
                        yield "{"
                        yield! seq {
                            yield sprintf "%s item = null;" defaultBaseClassName
                            yield "if (itemSyntax is StepSimpleItemSyntax simpleItem)"
                            yield "{"
                            yield! seq {
                                yield "switch (simpleItem.Keyword.ToUpperInvariant())"
                                yield "{"
                                yield! schema.Entities
                                        |> Seq.map (fun entity ->
                                            seq {
                                                yield sprintf "case \"%s\":" (entity.Name.ToUpperInvariant())
                                                yield! seq {
                                                    yield sprintf "item = %s.CreateFromSyntaxList(binder, simpleItem.Parameters);" (getIdentifierNameWithPrefix entity.Name typeNamePrefix)
                                                    yield "break;"
                                                } |> indentLines;
                                            })
                                        |> Seq.concat
                                        |> indentLines
                                yield! seq {
                                    yield "default:"
                                    yield! seq {
                                        yield "// TODO: track unsupported items"
                                        yield "break;"
                                    } |> indentLines
                                } |> indentLines
                                yield "}"
                            } |> indentLines
                            yield "}"
                            yield "// TODO: else"
                            yield ""
                            yield "return item;"
                        } |> indentLines
                        yield "}"
                    } |> indentLines
                    yield "}"
                } |> indentLines
                yield "}"
                yield ""
            } |> joinLines
        (itemName, content)
    let getAllFileDefinitions (schema: Schema) (generatedNamespace: string) (usingNamespaces: string seq) (typeNamePrefix: string) (defaultBaseClassName: string): (string * string) list =
        seq {
            yield getFromItemSyntaxFile schema generatedNamespace typeNamePrefix defaultBaseClassName
            yield! getEntityDefinitions schema generatedNamespace usingNamespaces typeNamePrefix defaultBaseClassName
        } |> List.ofSeq
