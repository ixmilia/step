// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using IxMilia.Step.Tokens;

namespace IxMilia.Step.Syntax
{
    internal class StepIntegerSyntax : StepSyntax
    {
        public override StepSyntaxType SyntaxType => StepSyntaxType.Integer;

        public int Value { get; }

        public StepIntegerSyntax(StepIntegerToken value)
            : base(value.Line, value.Column)
        {
            Value = value.Value;
        }

        public override IEnumerable<StepToken> GetTokens()
        {
            yield return new StepIntegerToken(Value, -1, -1);
        }
    }
}
