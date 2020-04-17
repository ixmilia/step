using System.Collections.Generic;
using System.Linq;
using IxMilia.Step.Tokens;

namespace IxMilia.Step.Syntax
{
    internal class StepComplexItemSyntax : StepItemSyntax
    {
        public override StepSyntaxType SyntaxType => StepSyntaxType.ComplexItem;

        public List<StepSimpleItemSyntax> Items { get; } = new List<StepSimpleItemSyntax>();

        public StepComplexItemSyntax(int line, int column, IEnumerable<StepSimpleItemSyntax> items)
            : base(line, column)
        {
            Items = items.ToList();
        }

        public override IEnumerable<StepToken> GetTokens()
        {
            return Items.SelectMany(i => i.GetTokens());
        }
    }
}
