// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using IxMilia.Step.Tokens;

namespace IxMilia.Step
{
    internal class StepIndividualValue : StepValue
    {
        public StepToken Value { get; }

        public StepIndividualValue(StepToken value)
            : base(value.Line, value.Column)
        {
            Value = value;
        }
    }
}
