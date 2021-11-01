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
    Assert.Equal("byte[]", getBaseTypeName (SimpleType(BinaryType(None, false))) "" Map.empty)
    Assert.Equal("bool", getBaseTypeName (SimpleType(BooleanType)) "" Map.empty)
    Assert.Equal("long", getBaseTypeName  (SimpleType(IntegerType)) "" Map.empty)
    Assert.Equal("bool", getBaseTypeName (SimpleType(LogicalType)) "" Map.empty)
    Assert.Equal("double", getBaseTypeName (SimpleType(NumberType)) "" Map.empty)
    Assert.Equal("double", getBaseTypeName (SimpleType(RealType(None))) "" Map.empty)
    Assert.Equal("string", getBaseTypeName (SimpleType(StringType(None, false))) "" Map.empty)
    Assert.Equal("TypePrefixSomeType", getBaseTypeName (NamedType "some_type") "TypePrefix" Map.empty)
    Assert.Equal("TypeFromTheOverrideMap", getBaseTypeName (NamedType "some_type") "TypePrefix" (Map.empty |> Map.add "some_type" (NamedType "TypeFromTheOverrideMap")))
    Assert.Equal("TypePrefixVector3D", getBaseTypeName (AggregationType(ListType(SimpleType(RealType(None)), LiteralValue(IntegerLiteral(0L)), Some(LiteralValue(IntegerLiteral(3L))), false))) "TypePrefix" Map.empty)

[<Fact>]
let ``schema type names``() =
    Assert.Equal("long", getSchemaTypeName (SchemaType("type_name", SimpleType(IntegerType), [])) "TypePrefix")
    Assert.Equal("TypePrefixTypeName", getSchemaTypeName (SchemaType("type_name", ConstructedType(EnumerationType([])), [])) "TypePrefix")

[<Fact>]
let ``entity declarations``() =
    Assert.Equal("public class TypePrefixShape : DefaultBaseEntity", getEntityDeclaration (Entity(EntityHead("shape", None, []), [ExplicitAttribute(ReferencedAttribute("size", None), AttributeType(SimpleType(RealType(None)), false))], [], [], [], [])) "TypePrefix" "DefaultBaseEntity")
    Assert.Equal("public class TypePrefixShape : TypePrefixParentEntity", getEntityDeclaration (Entity(EntityHead("shape", None, ["parent_entity"]), [ExplicitAttribute(ReferencedAttribute("size", None), AttributeType(SimpleType(RealType(None)), false))], [], [], [], [])) "TypePrefix" "DefaultBaseEntity")

[<Fact>]
let ``individual expressions as code``() =
    Assert.Equal(Some "(SomeField >= 0)", getExpressionCode (GreaterEquals(ReferencedAttributeExpression(ReferencedAttribute("some_field", None)), LiteralValue(RealLiteral(0.0)))))
    Assert.Equal(Some "(SomeField.SomeDeeperField >= 0)", getExpressionCode (GreaterEquals(ReferencedAttributeExpression(ReferencedAttribute("some_field", Some(ReferencedAttributeQualificationWithGroup(ReferencedAttribute("some_deeper_field", None))))), LiteralValue(RealLiteral(0.0)))))
    // any unsupported validation expression cancels the operation
    Assert.Equal(None, getExpressionCode (Greater(FunctionCallExpression(FunctionCall("some_function", [])), LiteralValue(RealLiteral(0.0)))))

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
let ``get type name overrides``() =
    Assert.Equal(Some(NamedType "double"), getNamedTypeOverride (SimpleType(RealType(None))))
    Assert.Equal(None, getNamedTypeOverride (NamedType "x"))

[<Fact>]
let ``entity definitions``() =
    let parentEntity = Entity(EntityHead("parent_entity", None, []), [], [], [], [], [])
    let entity = Entity(EntityHead("shape", None, ["parent_entity"]), [ExplicitAttribute(ReferencedAttribute("size", None), AttributeType(SimpleType(RealType(None)), false)); ExplicitAttribute(ReferencedAttribute("size2", None), AttributeType(NamedType "real_value", false))], [], [], [], [DomainRule("wr1", GreaterEquals(ReferencedAttributeExpression(ReferencedAttribute("size", None)), LiteralValue(RealLiteral(0.0))))])
    let schema = Schema("id", "version", SchemaBody([], [], [entity; parentEntity], []))
    let actual = getEntityDefinition schema entity "SomeNamespace" ["System"] "TypePrefix" "DefaultBaseClassName" (Map.empty |> Map.add "real_value" (NamedType "float")) |> snd
    Assert.Equal(@"using System;

namespace SomeNamespace
{
    public class TypePrefixShape : TypePrefixParentEntity
    {
        public override string ItemTypeString => ""SHAPE"";

        internal double _size;
        public double Size
        {
            get => _size;
            set
            {
                _size = value;
                ValidateDomainRules();
            }
        }

        internal float _size2;
        public float Size2
        {
            get => _size2;
            set
            {
                _size2 = value;
                ValidateDomainRules();
            }
        }


        internal TypePrefixShape()
        {
        }

        public TypePrefixShape(double size, float size2)
        {
            _size = size;
            _size2 = size2;
            ValidateDomainRules();
        }

        internal override void ValidateDomainRules()
        {
            base.ValidateDomainRules();
            if (!(Size >= 0))
            {
                throw new StepValidationException(""The validation rule 'wr1:size>=0' was not satisfied"");
            }
        }

        internal override IEnumerable<DefaultBaseClassName> GetReferencedItems()
        {
            yield break;
        }

        internal override IEnumerable<StepSyntax> GetParameters(StepWriter writer)
        {
            foreach (var parameter in base.GetParameters(writer))
            {
                yield return parameter;
            }

            yield return writer.GetItemSyntax(Size);
            yield return writer.GetItemSyntax(Size2);
        }

        internal static new TypePrefixShape CreateFromSyntaxList(StepBinder binder, StepSyntaxList syntaxList)
        {
            syntaxList.AssertListCount(2);
            var item = new TypePrefixShape();
            item._size = syntaxList.Values[0].GetDoubleValue();
            item._size2 = syntaxList.Values[1].GetFloatValue();
            return item;
        }
    }
}
".TrimStart().Replace("\r", ""), actual)

[<Fact>]
let ``generate item generator function``() =
    let parentEntity = Entity(EntityHead("parent_entity", None, []), [], [], [], [], [])
    let entity = Entity(EntityHead("shape", None, ["parent_entity"]), [], [], [], [], [])
    let schema = Schema("id", "version", SchemaBody([], [], [entity; parentEntity], []))
    let actual = getFromItemSyntaxFile schema "SomeNamespace" "TypePrefix" "DefaultBaseClassName" |> snd
    Assert.Equal(@"
using IxMilia.Step.Syntax;

namespace SomeNamespace
{
    internal static class DefaultBaseClassNameBuilder
    {
        internal static DefaultBaseClassName FromTypedParameter(StepBinder binder, StepItemSyntax itemSyntax)
        {
            DefaultBaseClassName item = null;
            if (itemSyntax is StepSimpleItemSyntax simpleItem)
            {
                switch (simpleItem.Keyword.ToUpperInvariant())
                {
                    case ""SHAPE"":
                        item = TypePrefixShape.CreateFromSyntaxList(binder, simpleItem.Parameters);
                        break;
                    case ""PARENT_ENTITY"":
                        item = TypePrefixParentEntity.CreateFromSyntaxList(binder, simpleItem.Parameters);
                        break;
                    default:
                        // TODO: track unsupported items
                        break;
                }
            }
            // TODO: else

            return item;
        }
    }
}
".TrimStart().Replace("\r", ""), actual)

//[<Fact>] // only used for diagnostics
let ``generate code for minimal schema``() =
    let schemaText = System.IO.File.ReadAllText("Schemas\minimal_201.exp")
    match run SchemaParser.parser schemaText with
    | Failure(errorMessage, _, _) -> failwith errorMessage
    | Success(schema, _, _) ->
        let generatedCode =
            System.String.Join("\n\n", getEntityDefinitions schema "SomeNamespace" ["System"] "Step" "StepItem" |> List.map snd)
        Assert.Equal("TODO:verify expected", generatedCode)

//[<Fact>] // only used for diagnostics
let ``parse full 201 schema``() =
    let schemaText = System.IO.File.ReadAllText(@"Schemas\ap201.exp")
    let parser = SchemaParser.parser
    match run parser schemaText with
    | Success(_result, _, _) -> printfn "File parsed successfully"
    | Failure(errorMessage, parserState, _) -> failwith <| sprintf "Parse failed at [%d, %d]: %s" parserState.Position.Line parserState.Position.Column errorMessage
