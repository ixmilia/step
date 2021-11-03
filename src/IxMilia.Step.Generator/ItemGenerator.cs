using System.Collections.Generic;
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
            var entityDefinitions = GenerateSource(schemaContent);
            foreach ((var entityName, var entityDefinition) in entityDefinitions)
            {
                context.AddSource(entityName, SourceText.From(entityDefinition, Encoding.UTF8));
            }
        }

        public static IEnumerable<(string name, string contents)> GenerateSource(string schemaContent)
        {
            var schema = SchemaParser.SchemaParser.RunParser(schemaContent);
            var entityDefinitions = CSharpSourceGenerator.getAllFileDefinitions(
                schema,
                generatedNamespace: "IxMilia.Step.Schemas.ExplicitDraughting",
                usingNamespaces: new[] { "System", "System.Collections.Generic", "System.Linq", "IxMilia.Step.Collections", "IxMilia.Step.Syntax" },
                typeNamePrefix: "Step",
                defaultBaseClassName: "StepItem");
            foreach (var entityDefinitionPair in entityDefinitions)
            {
                var entityName = entityDefinitionPair.Item1;
                var entityDefinition = entityDefinitionPair.Item2;
                yield return ($"{entityName}.Generated.cs", entityDefinition);
            }
        }
    }
}
