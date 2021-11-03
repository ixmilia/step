using System.Collections.Generic;
using System.IO;
using IxMilia.Step.SchemaParser;

namespace IxMilia.Step.Generator.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var assemblyDir = Path.GetDirectoryName(typeof(Program).Assembly.Location);
            var repoRoot = Path.Combine(assemblyDir, "..", "..", "..", "..", "..");
            var outputDir = Path.Combine(repoRoot, "src", "IxMilia.Step", "Schemas", "ExplicitDraughting", "Generated");
            var schemaContent = File.ReadAllText(Path.Combine(repoRoot, "src", "IxMilia.Step.SchemaParser.Test", "Schemas", "minimal_201.exp"));
            var entityDefinitions = GenerateSource(schemaContent);
            foreach ((var entityName, var entityDefinition) in entityDefinitions)
            {
                var outputPath = Path.Combine(outputDir, entityName);
                File.WriteAllText(outputPath, entityDefinition);
            }
        }

        private static IEnumerable<(string name, string contents)> GenerateSource(string schemaContent)
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
