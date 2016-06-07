// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using IxMilia.Step.Tokens;

namespace IxMilia.Step.Syntax
{
    internal class StepAutoSyntax : StepSyntax
    {
        public override StepSyntaxType SyntaxType => StepSyntaxType.Auto;

        public StepAsteriskToken Token { get; private set; }

        public StepAutoSyntax()
            : this(new StepAsteriskToken(-1, -1))
        {
        }

        public StepAutoSyntax(StepAsteriskToken token)
            : base(token.Line, token.Column)
        {
            Token = token;
        }

        public override IEnumerable<StepToken> GetTokens()
        {
            yield return Token;
        }
    }
}
