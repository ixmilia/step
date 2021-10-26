// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

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
        |> Array.map (fun p -> Char.ToUpperInvariant(p[0]).ToString() + p.Substring(1))
        |> Array.fold (+) ""
    let getParameterName (nameFromSchema: string) =
        let identifierName = getIdentifierName nameFromSchema
        Char.ToLowerInvariant(identifierName[0]).ToString() + identifierName.Substring(1)
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
    let getBaseTypeName (baseType: BaseType) (typeNamePrefix: string) (typeNameOverrides: Map<string, string>) =
        match baseType with
        | ConstructedType c -> "TODO"
        | AggregationType a -> "TODO"
        | SimpleType s -> getSimpleTypeName s typeNamePrefix
        | NamedType n ->
            match Map.tryFind n typeNameOverrides with
            | Some typeNameOverride -> typeNameOverride
            | None -> getIdentifierNameWithPrefix n typeNamePrefix
    let getTypeNameOverride (typ: BaseType): string option =
        match typ with
        | SimpleType s -> Some(getSimpleTypeName s "")
        | _ -> None
    let getTypeNameOverrideMap (types: SchemaType list): Map<string, string> =
        types
        |> List.map (fun t -> (t, getTypeNameOverride t.Type))
        |> List.filter (snd >> Option.isSome)
        |> List.map (fun (a, b) -> (a.Name, Option.get b))
        |> Map.ofList
    let private getExplicitAttributeDeclaration (attr: ExplicitAttribute) (typeNamePrefix: string) (typeNameOverrides: Map<string, string>) =
        let attributeType = getBaseTypeName attr.Type.Type typeNamePrefix typeNameOverrides
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
            | st::[] -> Some(getIdentifierNameWithPrefix st typeNamePrefix)
            | _ -> failwith "multiple inheritance NYI"
        let subTypeDefinitionText =
            match subTypeDefinition with
            | Some d -> sprintf " : %s" d
            | None -> ""
        sprintf "public class %s%s" entityName subTypeDefinitionText
    let getEntityDefinition (entity: Entity) (typeNamePrefix: string) (defaultBaseClassName: string option) (typeNameOverrides: Map<string, string>): string =
        let entityDeclaration = getEntityDeclaration entity typeNamePrefix defaultBaseClassName
        let allAttributeLines =
            entity.Attributes
            |> List.map (fun a -> getExplicitAttributeDeclaration a typeNamePrefix typeNameOverrides)
            |> joinWith "\n\n"
            |> toLines
            |> indentLines
        let validationRuleFunction = getDomainRuleValidationFunction entity.DomainRules |> toLines |> indentLines |> joinLines
        let argumentTexts =
            entity.Attributes
            |> List.map (fun a -> sprintf "%s %s" (getBaseTypeName a.Type.Type typeNamePrefix typeNameOverrides) (a.AttributeDeclaration.Name |> getParameterName))
        let allArgumentsText = String.Join(", ", argumentTexts)
        let fieldAssignmentLines =
            entity.Attributes
            |> List.map (fun a -> sprintf "%s = %s;" (getFieldName a.AttributeDeclaration.Name) (getParameterName a.AttributeDeclaration.Name))
        let constructorLines =
            seq {
                yield sprintf "public %s(%s)" (getIdentifierNameWithPrefix entity.Name typeNamePrefix) allArgumentsText
                yield "{"
                yield! fieldAssignmentLines |> indentLines
                yield "ValidateDomainRules();" |> indentLine
                yield "}"
            } |> indentLines
        seq {
            yield entityDeclaration
            yield "{"
            yield! allAttributeLines
            yield ""
            yield! constructorLines
            yield ""
            yield validationRuleFunction
            yield "}"
        } |> joinLines
    let getEntityDefinitions (schema: Schema) (typeNamePrefix: string) (defaultBaseClassName: string option): string list =
        let typeNameOverrides = getTypeNameOverrideMap schema.Types
        schema.Entities
        |> List.map (fun e -> getEntityDefinition e typeNamePrefix defaultBaseClassName typeNameOverrides)
