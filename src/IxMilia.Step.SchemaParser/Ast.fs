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

type AttributeReference =
    | LocalAttribute of string
    | QualifiedAttribute of string * string
    | SelfQualifiedAttribute of string * string

type Expression =
    // static values
    | LiteralValue of LiteralValue
    | AttributeExpression of AttributeReference
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
    // functions
    | FunctionCallExpression of FunctionCall
    | QueryExpression of Query
    // other
    | In of Expression * Expression
    | Or of Expression * Expression
    | Xor of Expression * Expression
    | And of Expression * Expression

and FunctionCall(name:string, arguments:Expression list) =
    member this.Name = name
    member this.Arguments = arguments
    override this.GetHashCode() = hash (this.Name, this.Arguments)
    override this.Equals(other) =
        match other with
        | :? FunctionCall as f -> (this.Name, this.Arguments) = (f.Name, f.Arguments)
        | _ -> false

and Query(variableId:string, aggregateSource:Expression, logicalExpression:Expression) =
    member this.VariableId = variableId
    member this.AggregateSource = aggregateSource
    member this.LogicalExpression = logicalExpression
    override this.GetHashCode() = hash (this.VariableId, this.AggregateSource, this.LogicalExpression)
    override this.Equals(other) =
        match other with
        | :? Query as q -> (this.VariableId, this.AggregateSource, this.LogicalExpression) = (q.VariableId, q.AggregateSource, q.LogicalExpression)
        | _ -> false

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

type UniqueRule(label:string, attributes:AttributeReference list) =
    member this.Label = label
    member this.Attributes = attributes

type InverseCollectionType =
    | Set
    | Bag

type InverseAttribute(name:string, collectionType:InverseCollectionType option, lowerBound:Expression option, upperBound:Expression option, entityName:string, attributeName:string) =
    member this.Name = name
    member this.CollectionType = collectionType
    member this.LowerBound = lowerBound
    member this.UpperBound = upperBound
    member this.EntityName = entityName
    member this.AttributeName = attributeName

type DerivedAttribute(name:string, typ:AttributeType, expression:Expression) =
    member this.Name = name
    member this.Type = typ
    member this.Expression = expression

type ExplicitAttribute(name:string, typ:AttributeType) =
    member this.Name = name
    member this.Type = typ

type DomainRule(label:string, expression:Expression) =
    member this.Label = label
    member this.Expression = expression

type SuperTypeExpressionItemType =
    | SuperTypeAnd
    | SuperTypeAndOr

type SuperTypeExpressionItem =
    | SuperTypeExpressionItem of SuperTypeExpressionItemType * SuperTypeFactor

and SuperTypeExpression =
    | SuperTypeExpression of SuperTypeExpressionItem list

and SuperTypeFactor =
    | SuperTypeEntityReference of string
    | SuperTypeOneOfEntityReference of string list
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
