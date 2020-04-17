using System.Collections.Generic;
using IxMilia.Step.Tokens;

namespace IxMilia.Step.Syntax
{
    internal class StepSimpleItemSyntax : StepItemSyntax
    {
        public override StepSyntaxType SyntaxType => StepSyntaxType.SimpleItem;

        public string Keyword { get; }
        public StepSyntaxList Parameters { get; }

        public StepSimpleItemSyntax(string keyword, StepSyntaxList parameters)
            : base(-1, -1)
        {
            Keyword = keyword;
            Parameters = parameters;
        }

        public StepSimpleItemSyntax(StepKeywordToken keyword, StepSyntaxList parameters)
            : base(keyword.Line, keyword.Column)
        {
            Keyword = keyword.Value;
            Parameters = parameters;
        }

        public override IEnumerable<StepToken> GetTokens()
        {
            yield return new StepKeywordToken(Keyword, -1, -1);
            foreach (var token in Parameters.GetTokens())
            {
                yield return token;
            }
        }
    }
}
