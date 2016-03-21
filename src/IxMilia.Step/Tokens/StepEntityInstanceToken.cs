// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace IxMilia.Step.Tokens
{
    internal class StepEntityInstanceToken : StepToken
    {
        public override StepTokenKind Kind => StepTokenKind.EntityInstance;

        public int Id { get; }

        public StepEntityInstanceToken(int id, int line, int column)
            : base(line, column)
        {
            Id = id;
        }
    }
}
