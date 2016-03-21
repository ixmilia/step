// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace IxMilia.Step.Tokens
{
    internal class StepRealToken : StepToken
    {
        public override StepTokenKind Kind => StepTokenKind.Real;

        public double Value { get; }

        public StepRealToken(double value, int line, int column)
            : base(line, column)
        {
            Value = value;
        }
    }
}
