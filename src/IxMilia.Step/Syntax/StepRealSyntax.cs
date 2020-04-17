using System.Collections.Generic;
using IxMilia.Step.Tokens;

namespace IxMilia.Step.Syntax
{
    internal class StepRealSyntax : StepSyntax
    {
        public override StepSyntaxType SyntaxType => StepSyntaxType.Real;

        public double Value { get; }

        public StepRealSyntax(double value)
            : base(-1, -1)
        {
            Value = value;
        }

        public StepRealSyntax(StepRealToken value)
            : base(value.Line, value.Column)
        {
            Value = value.Value;
        }

        public override IEnumerable<StepToken> GetTokens()
        {
            yield return new StepRealToken(Value, -1, -1);
        }
    }
}
