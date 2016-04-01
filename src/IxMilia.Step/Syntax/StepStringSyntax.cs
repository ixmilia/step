// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using IxMilia.Step.Tokens;

namespace IxMilia.Step.Syntax
{
    internal class StepStringSyntax : StepSyntax
    {
        public override StepSyntaxType SyntaxType => StepSyntaxType.String;

        public string Value { get; }

        public StepStringSyntax(string value)
            : base(-1, -1)
        {
            Value = value;
        }

        public StepStringSyntax(StepStringToken value)
            : base(value.Line, value.Column)
        {
            Value = value.Value;
        }

        public override string ToString(StepWriter writer)
        {
            return writer.ToString(Value);
        }
    }
}
