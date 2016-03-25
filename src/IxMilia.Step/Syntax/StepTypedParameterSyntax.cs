// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using IxMilia.Step.Tokens;

namespace IxMilia.Step.Syntax
{
    internal class StepTypedParameterSyntax : StepSyntax
    {
        public override StepSyntaxType SyntaxType => StepSyntaxType.TypedParameter;

        public string Keyword { get; }
        public StepSyntaxList Parameters { get; }

        public StepTypedParameterSyntax(StepKeywordToken keyword, StepSyntaxList parameters)
            : base(keyword.Line, keyword.Column)
        {
            Keyword = keyword.Value;
            Parameters = parameters;
        }
    }
}
