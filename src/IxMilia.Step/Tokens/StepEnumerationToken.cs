// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace IxMilia.Step.Tokens
{
    internal class StepEnumerationToken : StepToken
    {
        public override StepTokenKind Kind => StepTokenKind.Enumeration;

        public string Value { get; }

        public StepEnumerationToken(string value, int line, int column)
            : base(line, column)
        {
            Value = value;
        }
    }
}
