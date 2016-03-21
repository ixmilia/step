// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace IxMilia.Step.Tokens
{
    internal class StepSemiColonToken : StepToken
    {
        public override StepTokenKind Kind => StepTokenKind.SemiColon;

        public StepSemiColonToken(int line, int column)
            : base(line, column)
        {
        }
    }
}
