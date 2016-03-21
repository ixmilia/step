// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace IxMilia.Step.Tokens
{
    internal class StepConstantValueToken : StepToken
    {
        public override StepTokenKind Kind => StepTokenKind.ConstantValue;

        public string Name { get; }

        public StepConstantValueToken(string name, int line, int column)
            : base(line, column)
        {
            Name = name;
        }
    }
}
