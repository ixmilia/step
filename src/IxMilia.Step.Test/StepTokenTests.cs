using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using IxMilia.Step.Tokens;
using Xunit;

namespace IxMilia.Step.Test
{
    public class StepTokenTests : StepTestBase
    {
        private StepToken[] GetTokens(string text)
        {
            using (var stream = new MemoryStream())
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(text);
                writer.Flush();
                stream.Seek(0, SeekOrigin.Begin);
                var tokenizer = new StepTokenizer(stream);
                return tokenizer.GetTokens().ToArray();
            }
        }

        delegate bool TokenVerifier(StepToken token);

        private TokenVerifier WithKind(StepTokenKind kind)
        {
            return token => token.Kind == kind;
        }

        private TokenVerifier Semicolon()
        {
            return WithKind(StepTokenKind.Semicolon);
        }

        private TokenVerifier Keyword(string keyword)
        {
            return token => WithKind(StepTokenKind.Keyword)(token) && ((StepKeywordToken)token).Value == keyword;
        }

        private TokenVerifier Enumeration(string enumName)
        {
            return token => WithKind(StepTokenKind.Enumeration)(token) && ((StepEnumerationToken)token).Value == enumName;
        }

        private TokenVerifier Real(double value)
        {
            return token => WithKind(StepTokenKind.Real)(token) && ((StepRealToken)token).Value == value;
        }

        private TokenVerifier Integer(int value)
        {
            return token => WithKind(StepTokenKind.Integer)(token) && ((StepIntegerToken)token).Value == value;
        }

        private void VerifyTokens(string text, params TokenVerifier[] expected)
        {
            var actual = GetTokens(text);
            var upper = Math.Min(actual.Length, expected.Length);
            for (int i = 0; i < upper; i++)
            {
                Assert.True(expected[i](actual[i]));
            }
        }

        [Fact]
        public void SpecialTokenTest()
        {
            VerifyTokens("ISO-10303-21;",
                Keyword("ISO-10303-21"), Semicolon());
        }

        [Fact]
        public void EmptyFileTest()
        {
            VerifyTokens(@"
ISO-10303-21;
HEADER;
ENDSEC;
DATA;
ENDSEC;
END-ISO-10303-21;",
                Keyword("ISO-10303-21"), Semicolon(),
                Keyword("HEADER"), Semicolon(),
                Keyword("ENDSEC"), Semicolon(),
                Keyword("DATA"), Semicolon(),
                Keyword("ENDSEC"), Semicolon(),
                Keyword("END-ISO-10303-21"), Semicolon());
        }

        [Fact]
        public void TokensWithCommentsTest1()
        {
            VerifyTokens(@"
HEADER;
/* comment DATA; (comment's end is on the same line) */
ENDSEC;",
                Keyword("HEADER"), Semicolon(),
                Keyword("ENDSEC"), Semicolon());
        }

        [Fact]
        public void TokensWithCommentsTest2()
        {
            VerifyTokens(@"
HEADER;
/* comment (comment's end is on another line)
DATA; */
ENDSEC;",
                Keyword("HEADER"), Semicolon(),
                Keyword("ENDSEC"), Semicolon());
        }

        [Fact]
        public void TokensWithCommentsTest3()
        {
            VerifyTokens(@"
HEADER;
                /* comment
                (comment's end is on another line and farther back)
DATA; */
ENDSEC;",
                Keyword("HEADER"), Semicolon(),
                Keyword("ENDSEC"), Semicolon());
        }

        [Fact]
        public void TokensWithEmptylines()
        {
            VerifyTokens(@"

HEADER;

ENDSEC;

",
                Keyword("HEADER"), Semicolon(),
                Keyword("ENDSEC"), Semicolon());
        }

        [Fact]
        public void ParseEnumTokensTest()
        {
            VerifyTokens(".SOME_ENUM_VALUE.", Enumeration("SOME_ENUM_VALUE"));
        }

        [Fact]
        public void ParseInvarianCultureTest()
        {
            var existingCulture = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("de-DE");
                VerifyTokens("1.8", Real(1.8));
                VerifyTokens("54", Integer(54));
            }
            finally
            {
                CultureInfo.CurrentCulture = existingCulture;
            }
        }

        [Fact]
        public void WriteTokensPastLineLengthTest()
        {
            var tokens = new List<StepToken>();
            var maxItems = 22;
            for (int i = 0; i < maxItems; i++)
            {
                tokens.Add(new StepRealToken(0.0, -1, -1));
                if (i < maxItems - 1)
                {
                    tokens.Add(StepCommaToken.Instance);
                }
            }

            // should wrap at 80 characters
            var writer = new StepWriter(null, false);
            var sb = new StringBuilder();
            writer.WriteTokens(tokens, sb);
            var expected = NormalizeLineEndings(@"
0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,0.0,
0.0,0.0
".Trim());
            Assert.Equal(expected, sb.ToString());
        }
    }
}
