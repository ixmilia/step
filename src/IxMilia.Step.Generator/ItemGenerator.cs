using System.IO;
using System.Linq;
using System.Text;
using IxMilia.Step.SchemaParser;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace IxMilia.Step.Generator
{
    [Generator]
    public class ItemGenerator : ISourceGenerator
    {
        public void Initialize(GeneratorInitializationContext context)
        {
        }

        public void Execute(GeneratorExecutionContext context)
        {
            var schemaContent = context.AdditionalFiles.Single(f => Path.GetFileName(f.Path) == "minimal_201.exp").GetText().ToString();
            var schema = SchemaParser.SchemaParser.RunParser(schemaContent);
            var entityDefinitions = CSharpSourceGenerator.getEntityDefinitions(
                schema,
                generatedNamespace: "IxMilia.Step.Schemas.ExplicitDraughting",
                usingNamespaces: new[] { "System" },
                typeNamePrefix: "Step",
                defaultBaseClassName: "StepItem");
            foreach (var entityDefinitionPair in entityDefinitions)
            {
                var entityName = entityDefinitionPair.Item1;
                var entityDefinition = entityDefinitionPair.Item2;
                context.AddSource($"{entityName}.Generated.cs", SourceText.From(entityDefinition, Encoding.UTF8));
            }
        }
    }
}
