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

type Expression =
    // static values
    | LiteralValue of LiteralValue
    | AttributeName of string
    // artithmetic
    | Negate of Expression
    | Add of Expression * Expression
    | Subtract of Expression * Expression
    | Multiply of Expression * Expression
    | Divide of Expression * Expression
    | Modulus of Expression * Expression
    | Exponent of Expression * Expression
    // other
    | Or of Expression * Expression
    | Xor of Expression * Expression
    | And of Expression * Expression

type AttributeType(name: string, isOptional:bool) =
    member this.TypeName = name
    member this.IsOptional = isOptional

type DerivedAttribute(name:string, typ:AttributeType, expression:Expression) =
    member this.Name = name
    member this.Type = typ
    member this.Expression = expression

type ExplicitAttribute(name:string, typ:AttributeType) =
    member this.Name = name
    member this.Type = typ

type Entity(name:string, attributes:ExplicitAttribute list, derivedAttributes:DerivedAttribute list) =
    member this.Name = name
    member this.Attributes = attributes
    member this.DerivedAttributes = derivedAttributes

type SchemaBody(interfaces:ReferenceClause list, entities:Entity list) =
    member this.Interfaces = interfaces
    member this.Entities = entities

type Schema(id:string, version:string, body:SchemaBody) =
    member this.Id = id
    member this.Version = version
    member this.Body = body
    member this.Entities = body.Entities
