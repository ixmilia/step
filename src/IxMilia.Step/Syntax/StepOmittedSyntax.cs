using System.Collections.Generic;
using IxMilia.Step.Tokens;

namespace IxMilia.Step.Syntax
{
    internal class StepOmittedSyntax : StepSyntax
    {
        public override StepSyntaxType SyntaxType => StepSyntaxType.Omitted;

        public StepOmittedSyntax(StepOmittedToken value)
            : base(value.Line, value.Column)
        {
        }

        public override IEnumerable<StepToken> GetTokens()
        {
            yield return StepOmittedToken.Instance;
        }
    }
}
