// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace IxMilia.Step.Tokens
{
    internal class StepKeywordToken : StepToken
    {
        public override StepTokenKind Kind => StepTokenKind.Keyword;

        public string Value { get; }

        public StepKeywordToken(string value, int line, int column)
            : base(line, column)
        {
            Value = value;
        }
    }
}
