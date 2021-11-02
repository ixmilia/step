using System.IO;
using IxMilia.Step.Generator;

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
            var entityDefinitions = ItemGenerator.GenerateSource(schemaContent);
            foreach ((var entityName, var entityDefinition) in entityDefinitions)
            {
                var outputPath = Path.Combine(outputDir, entityName);
                File.WriteAllText(outputPath, entityDefinition);
            }
        }
    }
}
