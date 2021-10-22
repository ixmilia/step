// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace IxMilia.Step.SchemaParser

type Resource(name:string, alias:string option) =
    member this.Name = name
    member this.Alias = alias

type ReferenceClause(schemaName:string, resources:Resource list option) =
    member this.SchemaName = schemaName
    member this.Resources = resources

type LiteralValue =
    | IntegerLiteral of int64
    | LogicalLiteral of bool option
    | RealLiteral of float
    | StringLiteral of string
    override this.ToString() =
        match this with
        | IntegerLiteral i -> i.ToString()
        | LogicalLiteral l -> l.ToString()
        | RealLiteral f -> f.ToString()
        | StringLiteral s ->
            let escaped =
                s.ToCharArray()
                |> Array.fold (fun sb c ->
                    let t =
                        match c with
                        | '"' -> "\\\""
                        | '\r' -> "\\\r"
                        | '\n' -> "\\\n"
                        | _ -> c.ToString()
                    sb + t) ""
            "\"" + escaped + "\""

/// Internal-only type used for the attribute expression parser
type ReferencedAttributeValue =
    | ReferencedAttributeValueRoot of string // attribute
    | ReferencedAttributeValueAttribute of string // .attribute
    | ReferencedAttributeValueGroup of string // \attribute
    override this.ToString() =
        match this with
        | ReferencedAttributeValueRoot a -> a
        | ReferencedAttributeValueAttribute a -> a
        | ReferencedAttributeValueGroup a -> a

type ReferencedAttribute(attributeName:string, furtherQualification:ReferencedAttributeQualification option) =
    member _.Name = attributeName
    member _.Qualification = furtherQualification
    override this.GetHashCode() = hash (this.Name, this.Qualification)
    override this.Equals(other) =
        match other with
        | :? ReferencedAttribute as r ->
            let qualificationMatch =
                match (this.Qualification, r.Qualification) with
                | (Some a, Some b) -> a.Equals(b)
                | (None, None) -> true
                | _ -> false
            this.Name = r.Name && qualificationMatch
        | _ -> false
    override this.ToString() =
        let tail = match furtherQualification with
                   | Some q -> q.ToString()
                   | None -> ""
        attributeName + tail

and [<CustomEquality; NoComparison>] ReferencedAttributeQualification =
    | ReferencedAttributeQualificationWithAttribute of ReferencedAttribute // parent.reference
    | ReferencedAttributeQualificationWithGroup of ReferencedAttribute // parent\reference
    override this.GetHashCode() =
        match this with
        | ReferencedAttributeQualificationWithAttribute a -> hash a
        | ReferencedAttributeQualificationWithGroup a -> hash a
    override this.Equals(other) =
        match other with
        | :? ReferencedAttributeQualification as r ->
            match (this, r) with
            | (ReferencedAttributeQualificationWithAttribute a, ReferencedAttributeQualificationWithAttribute b) -> a.Equals(b)
            | (ReferencedAttributeQualificationWithGroup a, ReferencedAttributeQualificationWithGroup b) -> a.Equals(b)
            | _ -> false
        | _ -> false
    override this.ToString() =
        match this with
        | ReferencedAttributeQualificationWithAttribute r -> "." + r.ToString()
        | ReferencedAttributeQualificationWithGroup r -> "\\" + r.ToString()

type Expression =
    // static values
    | LiteralValue of LiteralValue
    // member access
    | ReferencedAttributeExpression of ReferencedAttribute
    // artithmetic
    | Negate of Expression
    | Add of Expression * Expression
    | Subtract of Expression * Expression
    | Multiply of Expression * Expression
    | Divide of Expression * Expression
    | Modulus of Expression * Expression
    | Exponent of Expression * Expression
    // logical
    | Greater of Expression * Expression
    | GreaterEquals of Expression * Expression
    | Less of Expression * Expression
    | LessEquals of Expression * Expression
    | Equals of Expression * Expression
    | NotEquals of Expression * Expression
    | Assignable of Expression * Expression * bool // _ * _ * is_strict
    | NotAssignable of Expression * Expression
    // functions
    | FunctionCallExpression of FunctionCall
    | QueryExpression of Query
    // other
    | ArrayExpression of Expression list
    | SubcomponentQualifiedExpression of Expression * Expression * Expression option // expression * index1 * index2
    | In of Expression * Expression
    | Or of Expression * Expression
    | Xor of Expression * Expression
    | And of Expression * Expression
    | Not of Expression
    override this.ToString() =
        match this with
        | LiteralValue l -> l.ToString()
        | ReferencedAttributeExpression r -> r.ToString()
        | Negate n -> "-" + n.ToString()
        | Add (a, b) -> a.ToString() + "+" + b.ToString()
        | Subtract (a, b) -> a.ToString() + "-" + b.ToString()
        | Multiply (a, b) -> a.ToString() + "*" + b.ToString()
        | Divide (a, b) -> a.ToString() + "/" + b.ToString()
        | Modulus (a, b) -> a.ToString() + "%" + b.ToString()
        | Exponent (a, b) -> a.ToString() + "^" + b.ToString()
        | Greater (a, b) -> a.ToString() + ">" + b.ToString()
        | GreaterEquals (a, b) -> a.ToString() + ">=" + b.ToString()
        | Less (a, b) -> a.ToString() + "<" + b.ToString()
        | LessEquals (a, b) -> a.ToString() + "<=" + b.ToString()
        | Equals (a, b) -> a.ToString() + "=" + b.ToString()
        | NotEquals (a, b) -> a.ToString() + "<>" + b.ToString()
        | Assignable (a, b, false) -> a.ToString() + ":=" + b.ToString()
        | Assignable (a, b, true) -> a.ToString() + ":=:" + b.ToString()
        | NotAssignable (a, b) -> a.ToString() + ":<>:" + b.ToString()
        | FunctionCallExpression f -> f.ToString()
        | QueryExpression q -> q.ToString()
        | ArrayExpression a -> "[" + System.String.Join(", ", a) + "]"
        | SubcomponentQualifiedExpression (a, b, c) ->
            let tail = match c with
                       | Some s -> ":" + s.ToString()
                       | None -> ""
            a.ToString() + "[" + b.ToString() + tail + "]"
        | In (a, b) -> a.ToString() + " in " + b.ToString()
        | Or (a, b) -> a.ToString() + " or " + b.ToString()
        | Xor (a, b) -> a.ToString() + " xor " + b.ToString()
        | And (a, b) -> a.ToString() + " and " + b.ToString()
        | Not e -> "not " + e.ToString()

and FunctionCall(name:string, arguments:Expression list) =
    member this.Name = name
    member this.Arguments = arguments
    override this.GetHashCode() = hash (this.Name, this.Arguments)
    override this.Equals(other) =
        match other with
        | :? FunctionCall as f -> (this.Name, this.Arguments) = (f.Name, f.Arguments)
        | _ -> false
    override this.ToString() =
        name + "(" + System.String.Join(", ", arguments) + ")"

and Query(variableId:string, aggregateSource:Expression, logicalExpression:Expression) =
    member this.VariableId = variableId
    member this.AggregateSource = aggregateSource
    member this.LogicalExpression = logicalExpression
    override this.GetHashCode() = hash (this.VariableId, this.AggregateSource, this.LogicalExpression)
    override this.Equals(other) =
        match other with
        | :? Query as q -> (this.VariableId, this.AggregateSource, this.LogicalExpression) = (q.VariableId, q.AggregateSource, q.LogicalExpression)
        | _ -> false
    override this.ToString() =
        "query(" + variableId + " <* " + aggregateSource.ToString() + " | " + logicalExpression.ToString() + ")"

type SimpleType =
    | BinaryType of Expression option * bool // width * isFixed
    | BooleanType
    | IntegerType
    | LogicalType
    | NumberType
    | RealType of Expression option // precision
    | StringType of Expression option * bool // width * isFixed

type ConstructedType =
    | EnumerationType of string list
    | SelectType of BaseType list

and AggregationType =
    | ArrayType of BaseType * Expression * Expression option * bool * bool // type * lowerBound * upperBound * isOptional * isUnique
    | BagType of BaseType * Expression * Expression option // type * lowerBound * upperBound
    | ListType of BaseType * Expression * Expression option * bool // type * lowerBound * upperBound * isUnique
    | SetType of BaseType * Expression * Expression option // type * lowerBound * upperBound

and BaseType =
    | ConstructedType of ConstructedType
    | AggregationType of AggregationType
    | SimpleType of SimpleType
    | NamedType of string

type AttributeType(typ:BaseType, isOptional:bool) =
    member this.Type = typ
    member this.IsOptional = isOptional

type UniqueRule(label:string, refAttrs:ReferencedAttribute list) =
    member this.Label = label
    member this.ReferencedAttributes = refAttrs

type InverseCollectionType =
    | Set
    | Bag

type InverseAttribute(name:string, collectionType:InverseCollectionType option, lowerBound:Expression option, upperBound:Expression option, entityName:string, attRef:string) =
    member this.Name = name
    member this.CollectionType = collectionType
    member this.LowerBound = lowerBound
    member this.UpperBound = upperBound
    member this.EntityName = entityName
    member this.AttributeReference = attRef

type DerivedAttribute(attDecl:ReferencedAttribute, typ:AttributeType, expression:Expression) =
    member this.AttributeDeclaration = attDecl
    member this.Type = typ
    member this.Expression = expression

type ExplicitAttribute(attDecl:ReferencedAttribute, typ:AttributeType) =
    member this.AttributeDeclaration = attDecl
    member this.Type = typ

type DomainRule(label:string, expression:Expression) =
    member this.Label = label
    member this.Expression = expression

type SuperTypeExpression =
    | SuperTypeFactor of SuperTypeFactor
    | SuperTypeAnd of SuperTypeExpression * SuperTypeExpression
    | SuperTypeAndOr of SuperTypeExpression * SuperTypeExpression

and SuperTypeFactor =
    | SuperTypeEntityReference of string
    | SuperTypeOneOf of string list
    | SuperTypeFactorExpression of SuperTypeExpression

type SuperTypeDeclaration =
    | AbstractSuperType of SuperTypeExpression option
    | SuperType of SuperTypeExpression

type EntityHead(name:string, SuperType:SuperTypeDeclaration option, SubTypes:string list) =
    member this.Name = name
    member this.SuperType = SuperType
    member this.SubTypes = SubTypes

type Entity(head:EntityHead, attributes:ExplicitAttribute list, derivedAttributes:DerivedAttribute list, inverseAttributes:InverseAttribute list, uniqueRestrictions:UniqueRule list, domainRules:DomainRule list) =
    member this.Head = head
    member this.Name = head.Name
    member this.SuperType = head.SuperType
    member this.SubTypes = head.SubTypes
    member this.Attributes = attributes
    member this.DerivedAttributes = derivedAttributes
    member this.InverseAttributes = inverseAttributes
    member this.UniqueRestrictions = uniqueRestrictions
    member this.DomainRules = domainRules

type SchemaType(name:string, typ:BaseType, domainRules:DomainRule list) =
    member this.Name = name
    member this.Type = typ
    member this.DomainRules = domainRules

type Declaration =
    | EntityDeclaration of Entity
    | TypeDeclaration of SchemaType

type Constant(id:string, typ:BaseType, expression:Expression) =
    member this.Id = id
    member this.Type = typ
    member this.Expression = expression

type SchemaBody(interfaces:ReferenceClause list, constants:Constant list, entities:Entity list, types:SchemaType list) =
    member this.Interfaces = interfaces
    member this.Constants = constants
    member this.Entities = entities
    member this.Types = types

type Schema(id:string, version:string, body:SchemaBody) =
    member this.Id = id
    member this.Version = version
    member this.Body = body
    member this.Interfaces = body.Interfaces
    member this.Constants = body.Constants
    member this.Entities = body.Entities
    member this.Types = body.Types
