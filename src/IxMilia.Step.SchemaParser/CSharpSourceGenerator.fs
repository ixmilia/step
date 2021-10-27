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
    let private getExplicitAttributeDeclaration (attr: ExplicitAttribute) (typeNamePrefix: string) (namedTypeOverrides: Map<string, BaseType>) =
        let attributeType = getBaseTypeName attr.Type.Type typeNamePrefix namedTypeOverrides
        let attributeName = getIdentifierName attr.AttributeDeclaration.Name
        let fieldName = getFieldName attr.AttributeDeclaration.Name
        seq {
            yield sprintf "private %s %s;" attributeType fieldName
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
    let getValidationStatementPredicate (expression: Expression) =
        let rec getValidationStatementPredicate' (expression: Expression) =
            match expression with
            | LiteralValue l -> l.ToString()
            | ReferencedAttributeExpression r -> r.Name |> getIdentifierName // TODO: add qualifications
            | Negate n -> "-" + getValidationStatementPredicate' n
            | Add (a, b) -> sprintf "(%s + %s)" (getValidationStatementPredicate' a) (getValidationStatementPredicate' b)
            | Subtract (a, b) -> sprintf "(%s - %s)" (getValidationStatementPredicate' a) (getValidationStatementPredicate' b)
            | Multiply (a, b) -> sprintf "(%s * %s)" (getValidationStatementPredicate' a) (getValidationStatementPredicate' b)
            | Divide (a, b) -> sprintf "(%s / %s)" (getValidationStatementPredicate' a) (getValidationStatementPredicate' b)
            | Modulus (a, b) -> sprintf "(%s %% %s)" (getValidationStatementPredicate' a) (getValidationStatementPredicate' b)
            | Exponent (a, b) -> sprintf "Math.Pow(%s, %s)" (getValidationStatementPredicate' a) (getValidationStatementPredicate' b)
            | Greater (a, b) -> sprintf "(%s > %s)" (getValidationStatementPredicate' a) (getValidationStatementPredicate' b)
            | GreaterEquals (a, b) -> sprintf "(%s >= %s)" (getValidationStatementPredicate' a) (getValidationStatementPredicate' b)
            | Less (a, b) -> sprintf "(%s < %s)" (getValidationStatementPredicate' a) (getValidationStatementPredicate' b)
            | LessEquals (a, b) -> sprintf "(%s <= %s)" (getValidationStatementPredicate' a) (getValidationStatementPredicate' b)
            | Equals (a, b) -> sprintf "(%s == %s)" (getValidationStatementPredicate' a) (getValidationStatementPredicate' b)
            | NotEquals (a, b) -> sprintf "(%s != %s)" (getValidationStatementPredicate' a) (getValidationStatementPredicate' b)
            | Assignable (a, b, _isStrict) -> sprintf "(%s is %s)" (getValidationStatementPredicate' a) (getValidationStatementPredicate' b)
            | NotAssignable (a, b) -> sprintf "!(%s is %s)" (getValidationStatementPredicate' a) (getValidationStatementPredicate' b)
            | FunctionCallExpression f ->
                let clrFunction =
                    match f.Name.ToLowerInvariant() with
                    | "sqrt" -> "Math.Sqrt"
                    | _ -> f.Name
                sprintf "%s(%s)" clrFunction (f.Arguments |> List.map getValidationStatementPredicate' |> joinWith ", ")
            | QueryExpression _ -> failwith "query NYI"
            | ArrayExpression _ -> failwith "array NYI"
            | SubcomponentQualifiedExpression (_, _, _) -> failwith "subcomponent NYI"
            | In (_, _) -> failwith "in NYI"
            | Or (a, b) -> sprintf "(%s || %s)" (getValidationStatementPredicate' a) (getValidationStatementPredicate' b)
            | Xor (a, b) -> sprintf "(%s ^ %s)" (getValidationStatementPredicate' a) (getValidationStatementPredicate' b)
            | And (a, b) -> sprintf "(%s && %s)" (getValidationStatementPredicate' a) (getValidationStatementPredicate' b)
            | Not n -> sprintf "!(%s)" (getValidationStatementPredicate' n)
        getValidationStatementPredicate' expression
    let private getValidationStatementBody (domainRule: DomainRule) =
        let predicate = getValidationStatementPredicate domainRule.Expression
        let domainRuleText = domainRule.ToString()
        seq {
            yield sprintf "if (!%s)" predicate
            yield "{"
            yield sprintf "throw new StepValidationException(\"The validation rule '%s' was not satisfied\");" domainRuleText |> indentLine
            yield "}"
        } |> joinLines
    let private getDomainRuleValidationFunction (domainRules: DomainRule list) =
        let allDomainRules =
            domainRules
            |> List.map getValidationStatementBody
            |> List.map toLines
            |> Seq.concat
        seq {
            yield "protected override void ValidateDomainRules()"
            yield "{"
            yield "base.ValidateDomainRules();" |> indentLine
            yield! allDomainRules |> indentLines
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
    let getEntityDeclaration (entity: Entity) (typeNamePrefix: string) (defaultBaseClassName: string option): string =
        let entityName = getIdentifierNameWithPrefix entity.Name typeNamePrefix
        let subTypeDefinition =
            match entity.SubTypes with
            | [] -> defaultBaseClassName
            | subTypeName::[] -> Some(getIdentifierNameWithPrefix subTypeName typeNamePrefix)
            | _ -> failwith "multiple inheritance NYI"
        let subTypeDefinitionText =
            match subTypeDefinition with
            | Some d -> sprintf " : %s" d
            | None -> ""
        sprintf "public class %s%s" entityName subTypeDefinitionText
    let getEntityDefinition (schema: Schema option) (entity: Entity) (generatedNamespace: string) (usingNamespaces: string list) (typeNamePrefix: string) (defaultBaseClassName: string option) (namedTypeOverrides: Map<string, BaseType>): (string * string) =
        let entityDeclaration = getEntityDeclaration entity typeNamePrefix defaultBaseClassName
        let allAttributeLines =
            entity.Attributes
            |> List.map (fun a -> getExplicitAttributeDeclaration a typeNamePrefix namedTypeOverrides)
            |> joinWith "\n\n"
            |> toLines
            |> indentLines
        let validationRuleFunction = getDomainRuleValidationFunction entity.DomainRules |> toLines |> indentLines |> joinLines
        let rec getEntityAndParentAttributes (entity: Entity) =
            let parentAttributes =
                match (schema, entity.SubTypes) with
                | (_, []) -> []
                | (Some schema, subTypeName::[]) ->
                    let parentEntity = schema.Entities |> List.find (fun e -> e.Name = subTypeName)
                    let (p, pp) = getEntityAndParentAttributes parentEntity
                    List.append pp p
                | (None, _) -> []
                | (_, _) -> failwith "multiple inheritance nyi"
            (entity.Attributes, parentAttributes)
        let (thisEntityAttributes, parentEntityAttributes) = getEntityAndParentAttributes entity
        let getAttributesAsArgumentTexts (attributes: ExplicitAttribute list) =
            attributes
            |> List.map (fun a -> sprintf "%s %s" (getBaseTypeName a.Type.Type typeNamePrefix namedTypeOverrides) (a.AttributeDeclaration.Name |> getParameterName))
        let thisArgumentTexts = getAttributesAsArgumentTexts thisEntityAttributes
        let parentArgumentTexts = getAttributesAsArgumentTexts parentEntityAttributes
        let allArgumentsText = List.append parentArgumentTexts thisArgumentTexts |> joinWith ", "
        let fieldAssignmentLines =
            entity.Attributes
            |> List.map (fun a -> sprintf "%s = %s;" (getFieldName a.AttributeDeclaration.Name) (getParameterName a.AttributeDeclaration.Name))
        let constructorLines =
            seq {
                yield sprintf "public %s(%s)" (getIdentifierNameWithPrefix entity.Name typeNamePrefix) allArgumentsText
                yield sprintf "%s: base(%s)" indentation (joinWith ", " (parentEntityAttributes |> List.map (fun a -> a.AttributeDeclaration.Name) |> List.map getParameterName))
                yield "{"
                yield! fieldAssignmentLines |> indentLines
                yield "ValidateDomainRules();" |> indentLine
                yield "}"
            } |> indentLines
        let generatedCode =
            seq {
                yield! usingNamespaces |> List.map (fun ns -> sprintf "using %s;" ns)
                yield ""
                yield sprintf "namespace %s" generatedNamespace
                yield "{"
                yield! entityDeclaration |> toLines |> indentLines
                yield "{" |> indentLine
                yield! allAttributeLines |> indentLines
                yield ""
                yield! constructorLines |> indentLines
                yield ""
                yield! validationRuleFunction |> toLines |> indentLines
                yield "}" |> indentLine
                yield "}"
            } |> joinLines
        let entityName = getIdentifierNameWithPrefix entity.Name typeNamePrefix
        (entityName, generatedCode)
    let getEntityDefinitions (schema: Schema) (generatedNamespace: string) (usingNamespaces: string list) (typeNamePrefix: string) (defaultBaseClassName: string option): (string * string) list =
        let namedTypeOverrides = getNamedTypeOverrideMap schema.Types
        schema.Entities
        |> List.map (fun e -> getEntityDefinition (Some schema) e generatedNamespace usingNamespaces typeNamePrefix defaultBaseClassName namedTypeOverrides)
