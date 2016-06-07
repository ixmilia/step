// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace IxMilia.Step.Tokens
{
    internal class StepRightParenToken : StepToken
    {
        public override StepTokenKind Kind => StepTokenKind.RightParen;

        public StepRightParenToken(int line, int column)
            : base(line, column)
        {
        }

        public override string ToString()
        {
            return ")";
        }
    }
}
