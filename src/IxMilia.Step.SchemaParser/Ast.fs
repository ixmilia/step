// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace IxMilia.Step.SchemaParser

type Resource (name: string, alias: string option) =
    member this.Name = name
    member this.Alias = alias

type ReferenceClause (schemaName: string, resources: Resource list option) =
    member this.SchemaName = schemaName
    member this.Resources = resources

type AttributeType (name: string, isOptional: bool) =
    member this.TypeName = name
    member this.IsOptional = isOptional

type ExplicitAttribute (name: string, typ: AttributeType) =
    member this.Name = name
    member this.Type = typ

type Entity (name: string, attributes: ExplicitAttribute list) =
    member this.Name = name
    member this.Attributes = attributes

type SchemaBody (interfaces: ReferenceClause list, entities: Entity list) =
    member this.Interfaces = interfaces
    member this.Entities = entities

type Schema (id: string, version: string, body: SchemaBody) =
    member this.Id = id
    member this.Version = version
    member this.Body = body
    member this.Entities = body.Entities
