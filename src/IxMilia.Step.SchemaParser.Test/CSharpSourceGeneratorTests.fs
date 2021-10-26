// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

module CSharpSourceGeneratorTests

open FParsec
open IxMilia.Step.SchemaParser
open IxMilia.Step.SchemaParser.CSharpSourceGenerator
open Xunit

[<Fact>]
let ``identifier names``() =
    Assert.Equal("Identifier", getIdentifierName "identifier")
    Assert.Equal("MyIdentifier", getIdentifierName "my_identifier")
    Assert.Equal("MyIdentifier1", getIdentifierName "my_identifier_1")

[<Fact>]
let ``base type names``() =
    Assert.Equal("byte[]", getBaseTypeName (SimpleType(BinaryType(None, false))) "")
    Assert.Equal("bool", getBaseTypeName (SimpleType(BooleanType)) "")
    Assert.Equal("long", getBaseTypeName  (SimpleType(IntegerType)) "")
    Assert.Equal("bool", getBaseTypeName (SimpleType(LogicalType)) "")
    Assert.Equal("double", getBaseTypeName (SimpleType(NumberType)) "")
    Assert.Equal("double", getBaseTypeName (SimpleType(RealType(None))) "")
    Assert.Equal("string", getBaseTypeName (SimpleType(StringType(None, false))) "")
    Assert.Equal("TypePrefixSomeType", getBaseTypeName (NamedType "some_type") "TypePrefix")

[<Fact>]
let ``schema type names``() =
    Assert.Equal("long", getSchemaTypeName (SchemaType("type_name", SimpleType(IntegerType), [])) "TypePrefix")
    Assert.Equal("TypePrefixTypeName", getSchemaTypeName (SchemaType("type_name", ConstructedType(EnumerationType([])), [])) "TypePrefix")

[<Fact>]
let ``entity declarations``() =
    Assert.Equal("public class TypePrefixShape", getEntityDeclaration (Entity(EntityHead("shape", None, []), [ExplicitAttribute(ReferencedAttribute("size", None), AttributeType(SimpleType(RealType(None)), false))], [], [], [], [])) "TypePrefix" None)
    Assert.Equal("public class TypePrefixShape : DefaultBaseEntity", getEntityDeclaration (Entity(EntityHead("shape", None, []), [ExplicitAttribute(ReferencedAttribute("size", None), AttributeType(SimpleType(RealType(None)), false))], [], [], [], [])) "TypePrefix" (Some "DefaultBaseEntity"))
    Assert.Equal("public class TypePrefixShape : TypePrefixParentEntity", getEntityDeclaration (Entity(EntityHead("shape", None, ["parent_entity"]), [ExplicitAttribute(ReferencedAttribute("size", None), AttributeType(SimpleType(RealType(None)), false))], [], [], [], [])) "TypePrefix" (Some "DefaultBaseEntity"))

[<Fact>]
let ``individual expression predicates``() =
    Assert.Equal(@"(SomeField >= 0)", getValidationStatementPredicate (GreaterEquals(ReferencedAttributeExpression(ReferencedAttribute("some_field", None)), LiteralValue(RealLiteral(0.0)))))

[<Fact>]
let ``schema type definitions``() =
    Assert.Equal(None, getSchemaTypeDefinition (SchemaType("name", SimpleType(BinaryType(None, false)), [])) "TypePrefix")
    Assert.Equal(None, getSchemaTypeDefinition (SchemaType("name", SimpleType(BooleanType), [])) "TypePrefix")
    Assert.Equal(None, getSchemaTypeDefinition (SchemaType("name", SimpleType(IntegerType), [])) "TypePrefix")
    Assert.Equal(None, getSchemaTypeDefinition (SchemaType("name", SimpleType(LogicalType), [])) "TypePrefix")
    Assert.Equal(None, getSchemaTypeDefinition (SchemaType("name", SimpleType(NumberType), [])) "TypePrefix")
    Assert.Equal(None, getSchemaTypeDefinition (SchemaType("name", SimpleType(RealType(None)), [])) "TypePrefix")
    Assert.Equal(None, getSchemaTypeDefinition (SchemaType("name", SimpleType(StringType(None, false)), [])) "TypePrefix")
    Assert.Equal(Some "public enum TypePrefixSomeEnumeration\n{\n    Val1,\n    Val2,\n}\n", getSchemaTypeDefinition (SchemaType("some_enumeration", ConstructedType(EnumerationType(["val1"; "val2"])), [])) "TypePrefix")

[<Fact>]
let ``entity definitions``() =
    let entity = Entity(EntityHead("shape", None, ["parent_entity"]), [ExplicitAttribute(ReferencedAttribute("size", None), AttributeType(SimpleType(RealType(None)), false)); ExplicitAttribute(ReferencedAttribute("size2", None), AttributeType(SimpleType(RealType(None)), false))], [], [], [], [DomainRule("wr1", GreaterEquals(ReferencedAttributeExpression(ReferencedAttribute("size", None)), LiteralValue(RealLiteral(0.0))))])
    let actual = getEntityDefinition entity "TypePrefix" None
    Assert.Equal(@"
public class TypePrefixShape : TypePrefixParentEntity
{
    private double _size;
    public double Size
    {
        get => _size;
        set
        {
            _size = value;
            ValidateDomainRules();
        }
    }

    private double _size2;
    public double Size2
    {
        get => _size2;
        set
        {
            _size2 = value;
            ValidateDomainRules();
        }
    }

    public TypePrefixShape(double size, double size2)
    {
        _size = size;
        _size2 = size2;
        ValidateDomainRules();
    }

    protected override void ValidateDomainRules()
    {
        base.ValidateDomainRules();
        if (!(Size >= 0))
        {
            throw new StepValidationException(""The validation rule 'wr1:size>=0' was not satisfied"");
        }
    }
}".Trim().Replace("\r", ""), actual)

[<Fact(Skip = "Only used for diagnostics")>]
let ``generate code for minimal schema``() =
    let schemaText = System.IO.File.ReadAllText("minimal_201.exp")
    match run SchemaParser.parser schemaText with
    | Failure(errorMessage, _, _) -> failwith errorMessage
    | Success(schema, _, _) ->
        let generatedCode =
            schema.Entities
            |> List.map (fun e -> getEntityDefinition e "Step" (Some "StepItem"))
            |> List.fold (fun a b -> a + "\n" + b) ""
        Assert.Equal("TODO:verify expected", generatedCode)
