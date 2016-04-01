// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Linq;
using IxMilia.Step.Tokens;

namespace IxMilia.Step.Syntax
{
    internal class StepSimpleEntitySyntax : StepEntitySyntax
    {
        public override StepSyntaxType SyntaxType => StepSyntaxType.SimpleEntity;

        public string Keyword { get; }
        public StepSyntaxList Parameters { get; }

        public StepSimpleEntitySyntax(string keyword, StepSyntaxList parameters)
            : base(-1, -1)
        {
            Keyword = keyword;
            Parameters = parameters;
        }

        public StepSimpleEntitySyntax(StepKeywordToken keyword, StepSyntaxList parameters)
            : base(keyword.Line, keyword.Column)
        {
            Keyword = keyword.Value;
            Parameters = parameters;
        }

        public override string ToString(StepWriter writer)
        {
            return Keyword + Parameters.ToString(writer);
        }
    }
}
