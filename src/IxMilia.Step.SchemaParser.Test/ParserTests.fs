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

let parseExpr str =
    let schemaText = sprintf " SCHEMA s ; ENTITY e ; DERIVE d : STRING := %s ; END_ENTITY ; END_SCHEMA ; " str
    let schema = parse schemaText
    schema.Entities.Single().DerivedAttributes.Single().Expression

let seqEqual (a : 'a seq, b : 'a seq) =
    Assert.Equal(a.Count(), b.Count())
    Seq.zip a b
    |> Seq.map (fun (l, r) -> l = r)
    |> Seq.fold (&&) true
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
    Assert.Equal(Multiply(IdentifierExpression "size", IdentifierExpression "size"), derived.Expression)

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

[<Fact>]
let ``schema with constants``() =
    let schema = parse " SCHEMA s ; CONSTANT pi : REAL := 3.14 ; two : INTEGER := 2 ; END_CONSTANT ; END_SCHEMA ; "
    Assert.Equal(2, schema.Constants.Length)
    Assert.Equal("pi", schema.Constants.First().Id)
    Assert.Equal(SimpleType(RealType None), schema.Constants.First().Type)
    Assert.Equal(LiteralValue(RealLiteral 3.14), schema.Constants.First().Expression)
    Assert.Equal("two", schema.Constants.Last().Id)
    Assert.Equal(SimpleType IntegerType, schema.Constants.Last().Type)
    Assert.Equal(LiteralValue(IntegerLiteral 2L), schema.Constants.Last().Expression)

[<Fact>]
let ``schema with inverse attributes``() =
    let schema = parse " SCHEMA s ; ENTITY e ; value : INTEGER ; INVERSE i : BAG OF e FOR value ; END_ENTITY ; END_SCHEMA ; "
    let inverse = schema.Entities.Single().InverseAttributes.Single()
    Assert.Equal("i", inverse.Name)
    Assert.Equal(Some Bag, inverse.CollectionType)
    Assert.Equal(None, inverse.LowerBound)
    Assert.Equal(None, inverse.UpperBound)
    Assert.Equal("e", inverse.EntityName)
    Assert.Equal("value", inverse.AttributeName)

[<Fact>]
let ``entity with unique rules``() =
    let schema = parse " SCHEMA s ; ENTITY e ; UNIQUE label : SELF\\entity.attribute ; END_ENTITY ; END_SCHEMA ; "
    let restr = schema.Entities.Single().UniqueRestrictions.Single()
    Assert.Equal("label", restr.Label)
    Assert.Equal(DottedAccessExpression(GroupQualifiedAccessExpression(IdentifierExpression "SELF", IdentifierExpression "entity"), IdentifierExpression "attribute"), restr.Expressions.Single())

[<Fact>]
let ``entity with restriction``() =
    let schema = parse " SCHEMA s ; ENTITY e ; WHERE wr1 : SELF >= 0 ; END_ENTITY ; END_SCHEMA ; "
    let domainRule = schema.Entities.Single().DomainRules.Single()
    Assert.Equal("wr1", domainRule.Label)
    Assert.Equal(GreaterEquals(IdentifierExpression "SELF", LiteralValue(IntegerLiteral 0L)), domainRule.Expression)

[<Fact>]
let ``entity with multiple restrictions``() =
    let schema = parse " SCHEMA s ; ENTITY  e ; WHERE wr1 : SELF >= 0 ; wr2 : (SELF > 0) AND (SELF < 10) ; END_ENTITY ; END_SCHEMA ; "
    let domainRules = schema.Entities.Single().DomainRules
    Assert.Equal(2, domainRules.Length)
    Assert.Equal(GreaterEquals(IdentifierExpression "SELF", LiteralValue(IntegerLiteral 0L)), domainRules.First().Expression)
    Assert.Equal(And(Greater(IdentifierExpression "SELF", LiteralValue(IntegerLiteral 0L)), Less(IdentifierExpression "SELF", LiteralValue(IntegerLiteral 10L))), domainRules.Last().Expression)

[<Fact>]
let ``entity with subtype``() =
    let schema = parse " SCHEMA s ; ENTITY person SUBTYPE OF ( mammal ) ; name : STRING ; END_ENTITY ; END_SCHEMA ; "
    seqEqual(["mammal"], schema.Entities.Single().SubTypes)

[<Fact>]
let ``entity with single supertype``() =
    let schema = parse " SCHEMA s ; ENTITY mammal SUPERTYPE OF ( animal ) ; END_ENTITY ; END_SCHEMA ; "
    Assert.Equal(SuperType(SuperTypeFactor(SuperTypeEntityReference "animal")), schema.Entities.Single().SuperType.Value)

[<Fact>]
let ``entity with many supertypes``() =
    let schema = parse " SCHEMA s ; ENTITY mammal SUPERTYPE OF ( ONEOF ( animal , not_animal ) ) ; END_ENTITY ; END_SCHEMA ; "
    Assert.Equal(SuperType(SuperTypeFactor(SuperTypeOneOf(["animal"; "not_animal"]))), schema.Entities.Single().SuperType.Value)

[<Fact>]
let ``complex supertype``() =
    let schema = parse "SCHEMA s; ENTITY e SUPERTYPE OF (ONEOF (a, b, c, d) ANDOR f); END_ENTITY; END_SCHEMA;"
    Assert.Equal(SuperType(SuperTypeAndOr(SuperTypeFactor(SuperTypeOneOf(["a"; "b"; "c"; "d"])), SuperTypeFactor(SuperTypeEntityReference "f"))), schema.Entities.Single().SuperType.Value)

[<Fact>]
let ``single-quoted strings in expressions``() =
    let expr = parseExpr "' ; END_ENTITY'"
    Assert.Equal(LiteralValue(StringLiteral " ; END_ENTITY"), expr)

[<Fact>]
let ``expression with relative operators``() =
    // these are nonsensical, but valid according to the spec
    Assert.Equal(In(LiteralValue(RealLiteral 1.0), LiteralValue(RealLiteral 4.0)), parseExpr "1.0 IN 4.0")
    Assert.Equal(In(LiteralValue(StringLiteral "string"), FunctionCallExpression(FunctionCall("COS", [LiteralValue(RealLiteral 1.0)]))), parseExpr @"'string' IN COS(1.0)")

[<Fact>]
let ``expression with function``() =
    Assert.Equal(FunctionCallExpression(FunctionCall("COS", [LiteralValue(RealLiteral 2.0)])), parseExpr "COS(2.0)")
    Assert.Equal(FunctionCallExpression(FunctionCall("COS", [LiteralValue(RealLiteral 2.0)])), parseExpr " COS ( 2.0 ) ")

[<Fact>]
let ``expression with qualified attribute``() =
    Assert.Equal(DottedAccessExpression(GroupQualifiedAccessExpression(IdentifierExpression "SELF", IdentifierExpression "a"), IdentifierExpression "b"), parseExpr @"SELF\a.b")
    Assert.Equal(IdentifierExpression "a", parseExpr @"a")
    Assert.Equal(IdentifierExpression "SELF", parseExpr @"SELF")

[<Fact>]
let ``entity with complex restriction``() =
    let schema = parse "SCHEMA s ; ENTITY e ; WHERE wr1 : 'asdf.jkl' IN TYPEOF ( foo.bar ) ; END_ENTITY ; END_SCHEMA ; "
    let expr = schema.Entities.Single().DomainRules.Single().Expression
    Assert.Equal(In(LiteralValue(StringLiteral "asdf.jkl"), FunctionCallExpression(FunctionCall("TYPEOF", [DottedAccessExpression(IdentifierExpression "foo", IdentifierExpression "bar")]))), expr)

[<Fact>]
let ``parse query expression``() =
    let expr = parseExpr @"QUERY ( x <* SELF\a.b | x > 4 )"
    Assert.Equal(QueryExpression(Query("x", DottedAccessExpression(GroupQualifiedAccessExpression(IdentifierExpression "SELF", IdentifierExpression "a"), IdentifierExpression "b"), Greater(IdentifierExpression "x", LiteralValue(IntegerLiteral 4L)))), expr)

[<Fact>]
let ``parse expression index``() =
    let expr = parseExpr "x[4]"
    Assert.Equal(SubcomponentQualifiedExpression(IdentifierExpression "x", LiteralValue(IntegerLiteral 4L), None), expr)

[<Fact>]
let ``parse dot qualified expression after an index``() =
    let expr = parseExpr("x[4].foo")
    Assert.Equal(DottedAccessExpression(SubcomponentQualifiedExpression(IdentifierExpression "x", LiteralValue(IntegerLiteral 4L), None), IdentifierExpression "foo"), expr)

[<Fact>]
let ``parse array expression``() =
    let expr = parseExpr "['a', 'b']"
    Assert.Equal(ArrayExpression([LiteralValue(StringLiteral "a"); LiteralValue(StringLiteral "b")]), expr)

(*
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

// *)
