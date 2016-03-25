// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace IxMilia.Step.Syntax
{
    internal class StepEntityInstanceSyntax : StepSyntax
    {
        public override StepSyntaxType SyntaxType => StepSyntaxType.EntityInstance;

        public int Id { get; }
        public StepTypedParameterSyntax SimpleEntityInstance { get; }

        public StepEntityInstanceSyntax(int line, int column, int id, StepTypedParameterSyntax entityInstance)
            : base(line, column)
        {
            Id = id;
            SimpleEntityInstance = entityInstance;
        }
    }
}
