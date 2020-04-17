using System;
using System.Collections.Generic;
using System.Linq;
using IxMilia.Step.Tokens;

namespace IxMilia.Step.Syntax
{
    internal class StepDataSectionSyntax : StepSyntax
    {
        public override StepSyntaxType SyntaxType => StepSyntaxType.DataSection;

        public List<StepEntityInstanceSyntax> ItemInstances { get; }

        public StepDataSectionSyntax(int line, int column, IEnumerable<StepEntityInstanceSyntax> itemInstances)
            : base(line, column)
        {
            ItemInstances = itemInstances.ToList();
        }

        public override IEnumerable<StepToken> GetTokens()
        {
            throw new NotSupportedException();
        }
    }
}
