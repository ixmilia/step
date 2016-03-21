// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.IO;
using System.Linq;
using IxMilia.Step.Tokens;
using Xunit;

namespace IxMilia.Step.Test
{
    public class StepTokenTests
    {
        private StepToken[] GetTokens(string text)
        {
            var stream = new MemoryStream();
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(text);
                writer.Flush();
                var tokenizer = new StepTokenizer(stream);
                return tokenizer.GetTokens().ToArray();
            }
        }

        delegate bool TokenVerifier(StepToken token);

        private TokenVerifier WithKind(StepTokenKind kind)
        {
            return token => token.Kind == kind;
        }

        private TokenVerifier SemiColon()
        {
            return WithKind(StepTokenKind.SemiColon);
        }

        private TokenVerifier Keyword(string keyword)
        {
            return token => WithKind(StepTokenKind.Keyword)(token) && ((StepKeywordToken)token).Value == keyword;
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
                Keyword("ISO-10303-21"),
                SemiColon());
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
                Keyword("ISO-10303-21"), SemiColon(),
                Keyword("HEADER"), SemiColon(),
                Keyword("ENDSEC"), SemiColon(),
                Keyword("DATA"), SemiColon(),
                Keyword("ENDSEC"), SemiColon(),
                Keyword("END-ISO-10303-21"), SemiColon());
        }
    }
}
