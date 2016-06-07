// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace IxMilia.Step.Tokens
{
    internal class StepIntegerToken : StepToken
    {
        public override StepTokenKind Kind => StepTokenKind.Integer;

        public int Value { get; }

        public StepIntegerToken(int value, int line, int column)
            : base(line, column)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
