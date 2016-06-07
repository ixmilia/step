// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using IxMilia.Step.Tokens;

namespace IxMilia.Step.Syntax
{
    internal class StepEnumerationValueSyntax : StepSyntax
    {
        public override StepSyntaxType SyntaxType => StepSyntaxType.Enumeration;

        public string Value { get; }

        public StepEnumerationValueSyntax(string value)
            : base(-1, -1)
        {
            Value = value;
        }

        public StepEnumerationValueSyntax(StepEnumerationToken value)
            : base(value.Line, value.Column)
        {
            Value = value.Value;
        }

        public override IEnumerable<StepToken> GetTokens()
        {
            yield return new StepEnumerationToken(Value, -1, -1);
        }
    }
}
