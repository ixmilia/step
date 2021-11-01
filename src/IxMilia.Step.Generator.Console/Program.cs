using System.IO;
using IxMilia.Step.Generator;

namespace IxMilia.Step.Generator.Console
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var schemaContent = File.ReadAllText(Path.Combine("..", "..", "..", "..", "IxMilia.Step.SchemaParser.Test", "Schemas", "minimal_201.exp"));
            var entityDefinitions = ItemGenerator.GenerateSource(schemaContent);
            foreach ((var entityName, var entityDefinition) in entityDefinitions)
            {
                File.WriteAllText(entityName, entityDefinition);
            }
        }
    }
}
