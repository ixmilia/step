// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module ParserTests

open System.Linq
open IxMilia.Step.SchemaParser
open FParsec
open Xunit

let parse str =
    match run SchemaParser.parser str with
    | Success(result, _, _) -> result
    | Failure(errorMessage, _, _) -> failwith errorMessage

let arrayEqual (a : 'a array, b : 'a array) =
    Assert.Equal(a.Length, b.Length)
    Array.zip a b
    |> Array.map (fun (l, r) -> l = r)
    |> Array.fold (&&) true
    |> Assert.True

[<Fact>]
let ``empty schema``() =
    let schema = parse " SCHEMA test_schema1 ; END_SCHEMA ; "
    Assert.Equal("test_schema1", schema.Id)

[<Fact>]
let ``schema with version``() =
    let schema = parse " SCHEMA name \"version with \"\" double quote\" ; END_SCHEMA ; "
    Assert.Equal("version with \" double quote", schema.Version)


[<Fact>]
let ``empty entity``() =
    let schema = parse " SCHEMA test_schema ; ENTITY empty_entity ; END_ENTITY ; END_SCHEMA ; "
    Assert.Equal("empty_entity", schema.Entities.Single().Name)

[<Fact>]
let ``simple entity``() =
    let schema = parse " SCHEMA s ; ENTITY point ; x : REAL ; y : REAL ( 3 ) ; END_ENTITY ; END_SCHEMA ; "
    let entity = schema.Entities.Single()
    Assert.Equal(2, entity.Attributes.Length)
    Assert.Equal("x", entity.Attributes.[0].Name)
    Assert.Equal(SimpleType(RealType None), entity.Attributes.[0].Type.Type)
    Assert.Equal("y", entity.Attributes.[1].Name)
    Assert.Equal(SimpleType(RealType(Some(LiteralValue(IntegerLiteral 3L)))), entity.Attributes.[1].Type.Type)

[<Fact>]
let ``entity with binary type``() =
    let schema = parse " SCHEMA s ; ENTITY bitfields ; b : BINARY ( 3 ) FIXED ; END_ENTITY ; END_SCHEMA ; "
    let entity = schema.Entities.Single()
    Assert.Equal(SimpleType(BinaryType(Some(LiteralValue(IntegerLiteral 3L)), true)), entity.Attributes.[0].Type.Type)

[<Fact>]
let ``entity with boolean type``() =
    let schema = parse " SCHEMA s ; ENTITY e ; b : BOOLEAN ; END_ENTITY ; END_SCHEMA ; "
    let entity = schema.Entities.Single()
    Assert.Equal(SimpleType(BooleanType), entity.Attributes.[0].Type.Type)

[<Fact>]
let ``entity with integer type``() =
    let schema = parse " SCHEMA s ; ENTITY e ; i : INTEGER ; END_ENTITY ; END_SCHEMA ; "
    let entity = schema.Entities.Single()
    Assert.Equal(SimpleType(IntegerType), entity.Attributes.[0].Type.Type)

[<Fact>]
let ``entity with logical type``() =
    let schema = parse " SCHEMA s ; ENTITY e ; l : LOGICAL ; END_ENTITY ; END_SCHEMA ; "
    let entity = schema.Entities.Single()
    Assert.Equal(SimpleType(LogicalType), entity.Attributes.[0].Type.Type)

[<Fact>]
let ``entity with number type``() =
    let schema = parse " SCHEMA s ; ENTITY e ; n : NUMBER ; END_ENTITY ; END_SCHEMA ; "
    let entity = schema.Entities.Single()
    Assert.Equal(SimpleType(NumberType), entity.Attributes.[0].Type.Type)

[<Fact>]
let ``entity with string type``() =
    let schema = parse " SCHEMA s ; ENTITY e ; s : STRING ( 3 ) ; END_ENTITY ; END_SCHEMA ; "
    let entity = schema.Entities.Single()
    Assert.Equal(SimpleType(StringType(Some(LiteralValue(IntegerLiteral 3L)), false)), entity.Attributes.[0].Type.Type)

[<Fact>]
let ``entity with array type``() =
    let schema = parse " SCHEMA s ; ENTITY e ; a : ARRAY [ 2 : ? ] OF OPTIONAL UNIQUE BOOLEAN ; END_ENTITY ; END_SCHEMA ; "
    let entity = schema.Entities.Single()
    Assert.Equal(AggregationType(ArrayType(SimpleType(BooleanType), LiteralValue(IntegerLiteral 2L), None, true, true)), entity.Attributes.Single().Type.Type)

[<Fact>]
let ``entity with bag type``() =
    let schema = parse " SCHEMA s ; ENTITY e ; b : BAG [ 2 : ? ] OF BOOLEAN ; END_ENTITY ; END_SCHEMA ; "
    let entity = schema.Entities.Single()
    Assert.Equal(AggregationType(BagType(SimpleType(BooleanType), LiteralValue(IntegerLiteral 2L), None)), entity.Attributes.Single().Type.Type)

[<Fact>]
let ``entity with list type``() =
    let schema = parse " SCHEMA s ; ENTITY e ; l : LIST [ 2 : ? ] OF UNIQUE BOOLEAN ; END_ENTITY ; END_SCHEMA ; "
    let entity = schema.Entities.Single()
    Assert.Equal(AggregationType(ListType(SimpleType(BooleanType), LiteralValue(IntegerLiteral 2L), None, true)), entity.Attributes.Single().Type.Type)

[<Fact>]
let ``entity with set type``() =
    let schema = parse " SCHEMA s ; ENTITY e ; s : SET [ 2 : ? ] OF BOOLEAN ; END_ENTITY ; END_SCHEMA ; "
    let entity = schema.Entities.Single()
    Assert.Equal(AggregationType(SetType(SimpleType(BooleanType), LiteralValue(IntegerLiteral 2L), None)), entity.Attributes.Single().Type.Type)

[<Fact>]
let ``entity with nested aggretation types``() =
    let schema = parse " SCHEMA s ; ENTITY e ; b : BAG [ 2 : ? ] OF BAG [ 3 : ? ] OF BAG [ 4 : ? ] OF BOOLEAN ; END_ENTITY ; END_SCHEMA ; "
    let entity = schema.Entities.Single()
    Assert.Equal(AggregationType(BagType(AggregationType(BagType(AggregationType(BagType(SimpleType(BooleanType), LiteralValue(IntegerLiteral 4L), None)), LiteralValue(IntegerLiteral 3L), None)), LiteralValue(IntegerLiteral 2L), None)), entity.Attributes.Single().Type.Type)

[<Fact>]
let ``entity with optional parameter``() =
    let schema = parse " SCHEMA s ; ENTITY point ; x : REAL ; y : OPTIONAL REAL ; END_ENTITY ; END_SCHEMA ; "
    let entity = schema.Entities.Single()
    Assert.Equal(2, entity.Attributes.Length)
    Assert.Equal("x", entity.Attributes.[0].Name)
    Assert.Equal(SimpleType(RealType None), entity.Attributes.[0].Type.Type)
    Assert.False(entity.Attributes.[0].Type.IsOptional)
    Assert.Equal("y", entity.Attributes.[1].Name)
    Assert.Equal(SimpleType(RealType None), entity.Attributes.[1].Type.Type)
    Assert.True(entity.Attributes.[1].Type.IsOptional)

[<Fact>]
let ``entity attribute with non-built-in-type``() =
    let schema = parse " SCHEMA s ; ENTITY circle ; center : point ; END_ENTITY ; END_SCHEMA ; "
    Assert.Equal(NamedType "point", schema.Entities.Single().Attributes.Single().Type.Type)

[<Fact>]
let ``entity with derived``() =
    let schema = parse " SCHEMA s ; ENTITY square ; size : REAL ; DERIVE area : REAL := size * size ; END_ENTITY ; END_SCHEMA ; "
    let derived = schema.Entities.Single().DerivedAttributes.Single()
    Assert.Equal("area", derived.Name)
    Assert.Equal(Multiply(AttributeName "size", AttributeName "size"), derived.Expression)

[<Fact>]
let ``multiple entities``() =
    let schema = parse " SCHEMA s ; ENTITY a ; END_ENTITY ; ENTITY b ; END_ENTITY ; END_SCHEMA ; "
    Assert.Equal(2, schema.Entities.Length)
    Assert.Equal("a", schema.Entities.First().Name)
    Assert.Equal("b", schema.Entities.Last().Name)

[<Fact>]
let ``comments``() =
    let schema = parse @"
-- single line comment
SCHEMA s (* multi-line comment
*) ;
-- comment 1
-- comment 2
-- comment 3

(* another multi-line comment *)

END_SCHEMA ;
"
    Assert.Equal("s", schema.Id)

[<Fact>]
let ``simple types``() =
    let schema = parse " SCHEMA s ; TYPE length = REAL ; END_TYPE ; TYPE width = REAL ; END_TYPE ; END_SCHEMA ; "
    Assert.Equal(2, schema.Types.Length)
    Assert.Equal("length", schema.Types.First().Name)
    Assert.Equal(SimpleType(RealType None), schema.Types.First().Type)
    Assert.Equal("width", schema.Types.Last().Name)
    Assert.Equal(SimpleType(RealType None), schema.Types.Last().Type)

[<Fact>]
let ``type and entity``() =
    let schema = parse " SCHEMA s ; TYPE double = REAL ; END_TYPE ; ENTITY point ; END_ENTITY ; END_SCHEMA ; "
    Assert.Equal("double", schema.Types.Single().Name)
    Assert.Equal("point", schema.Entities.Single().Name)

    
[<Fact>]
let ``enumeration type``() =
    let schema = parse " SCHEMA s ; TYPE numbers = ENUMERATION OF ( uno , dos , tres ) ; END_TYPE ; END_SCHEMA ; "
    Assert.Equal(ConstructedType(EnumerationType(["uno"; "dos"; "tres"])), schema.Types.Single().Type)

[<Fact>]
let ``type with one select value``() =
    let schema = parse " SCHEMA s ; TYPE t = SELECT ( bar ) ; END_TYPE ; END_SCHEMA ; "
    Assert.Equal(ConstructedType(SelectType([NamedType "bar"])), schema.Types.Single().Type)

[<Fact>]
let ``type with two select values``() =
    let schema = parse " SCHEMA s ; TYPE t = SELECT ( bar , baz ) ; END_TYPE ; END_SCHEMA ; "
    Assert.Equal(ConstructedType(SelectType([NamedType "bar"; NamedType "baz"])), schema.Types.Single().Type)

(*
[<Fact>]
let ``type with restriction``() =
    let schema = parse " SCHEMA s ; TYPE measure = REAL ; WHERE wr1 : SELF >= 0 ; END_TYPE ; END_SCHEMA ; "
    Assert.Equal(TypeRestriction("wr1", GreaterEqual(Identifier("SELF"), Number(0.0))), schema.Types.Single().Restrictions.Single())

[<Fact>]
let ``type with multiple restrictions``() =
    let schema = parse " SCHEMA s ; TYPE measure = REAL ; WHERE wr1 : SELF >= 0 ; wr2 : (SELF > 0) AND (SELF < 10) ; END_TYPE ; END_SCHEMA ; "
    Assert.Equal(2, schema.Types.Single().Restrictions.Length)
    Assert.Equal(TypeRestriction("wr1", GreaterEqual(Identifier("SELF"), Number(0.0))), schema.Types.Single().Restrictions.First())
    Assert.Equal(TypeRestriction("wr2", And(Greater(Identifier("SELF"), Number(0.0)), Less(Identifier("SELF"), Number(10.0)))), schema.Types.Single().Restrictions.Last())

[<Fact>]
let ``type with function restriction``() =
    let schema = parse " SCHEMA s ; TYPE measure = REAL ; WHERE wr1 : EXISTS ( SELF ) OR FOO ( 1.2, 3.4 ) ; END_TYPE ; END_SCHEMA ; "
    Assert.Equal(TypeRestriction("wr1", Or(Function("EXISTS", [| Identifier("SELF") |]), Function("FOO", [| Number(1.2); Number(3.4) |]))), schema.Types.Single().Restrictions.Single())

[<Fact>]
let ``expression with an index``() =
    let schema = parse " SCHEMA s ; ENTITY e ; WHERE wr1 : foo[bar] > 0 ; END_ENTITY ; END_SCHEMA ; "
    Assert.Equal(Greater(Index("foo", [| Identifier("bar") |]), Number(0.0)), schema.Entities.Single().Restrictions.Single().Expression)

[<Fact>]
let ``entity with restriction``() =
    let schema = parse " SCHEMA s ; ENTITY person ; name : STRING ; alias : STRING ; WHERE wr1 : EXISTS ( name ) OR EXISTS ( alias ) ; END_ENTITY ; END_SCHEMA ; "
    Assert.Equal(TypeRestriction("wr1", Or(Function("EXISTS", [| Identifier("name") |]), Function("EXISTS", [| Identifier("alias") |]))), schema.Entities.Single().Restrictions.Single())

[<Fact>]
let ``entity with complex restriction``() =
    let schema = parse "SCHEMA s ; ENTITY e ; WHERE wr1 : 'asdf.jkl' IN TYPEOF ( SELF\\foo.bar ) ; END_ENTITY ; END_SCHEMA ; "
    Assert.Equal(In(String("asdf.jkl"), Function("TYPEOF", [| Identifier("SELF\\foo.bar") |])), schema.Entities.Single().Restrictions.Single().Expression)

[<Fact>]
let ``entity with no subtype or supertype``() =
    let schema = parse " SCHEMA s ; ENTITY person ; name : STRING ; END_ENTITY ; END_SCHEMA ; "
    Assert.Equal("", schema.Entities.Single().SubType)
    Assert.Empty(schema.Entities.Single().SuperTypes)

[<Fact>]
let ``entity with subtype``() =
    let schema = parse " SCHEMA s ; ENTITY person SUBTYPE OF ( mammal ) ; name : STRING ; END_ENTITY ; END_SCHEMA ; "
    Assert.Equal("mammal", schema.Entities.Single().SubType)

[<Fact>]
let ``entity with single supertype``() =
    let schema = parse " SCHEMA s ; ENTITY mammal SUPERTYPE OF ( animal ) ; END_ENTITY ; END_SCHEMA ; "
    Assert.Equal("animal", schema.Entities.Single().SuperTypes.Single())

[<Fact>]
let ``entity with many supertypes``() =
    let schema = parse " SCHEMA s ; ENTITY mammal SUPERTYPE OF ( ONEOF ( animal , not_animal ) ) ; END_ENTITY ; END_SCHEMA ; "
    arrayEqual([| "animal"; "not_animal" |], schema.Entities.Single().SuperTypes)

[<Fact>]
let ``entity property with upper/lower bounds``() =
    let schema = parse " SCHEMA s ; ENTITY e ; p : SET [ 2 : ? ] OF REAL ; END_ENTITY ; END_SCHEMA ; "
    let propertyType = schema.Entities.Single().Properties.Single().Type
    Assert.False(propertyType.IsOptional)
    Assert.Equal("REAL", propertyType.TypeName)
    Assert.Equal(Some(Some(2), None), propertyType.Bounds)

[<Fact>]
let ``expression with multi-line qualified identifier``() =
    let schema = parse " SCHEMA s ; ENTITY e ; WHERE wr1 : foo\\\nbar.\nbaz > 0 ; END_ENTITY ; END_SCHEMA ; "
    Assert.Equal(Greater(Identifier("foo\\bar.baz"), Number(0.0)), schema.Entities.Single().Restrictions.Single().Expression)

[<Fact>]
let ``expression with an array``() =
    let schema = parse " SCHEMA s ; ENTITY e ; WHERE wr1 : [ 'a' , 'b' ] ; END_ENTITY ; END_SCHEMA ; "
    Assert.Equal(Array([|String("a"); String("b")|]), schema.Entities.Single().Restrictions.Single().Expression)
// *)
