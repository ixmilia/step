using System.Collections.Generic;
using System.Linq;
using IxMilia.Step.Tokens;

namespace IxMilia.Step.Syntax
{
    internal class StepSyntaxList : StepSyntax
    {
        public override StepSyntaxType SyntaxType => StepSyntaxType.List;

        public List<StepSyntax> Values { get; }

        public StepSyntaxList(params StepSyntax[] values)
            : this(-1, -1, values)
        {
        }

        public StepSyntaxList(IEnumerable<StepSyntax> values)
            : this(-1, -1, values)
        {
        }

        public StepSyntaxList(int line, int column, IEnumerable<StepSyntax> values)
            : base(line, column)
        {
            Values = values.ToList();
        }

        public override IEnumerable<StepToken> GetTokens()
        {
            yield return StepLeftParenToken.Instance;
            for (int i = 0; i < Values.Count; i++)
            {
                foreach (var token in Values[i].GetTokens())
                {
                    yield return token;
                }

                if (i < Values.Count - 1)
                {
                    yield return StepCommaToken.Instance;
                }
            }

            yield return StepRightParenToken.Instance;
        }
    }
}
