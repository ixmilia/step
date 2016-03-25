// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;

namespace IxMilia.Step.Syntax
{
    internal class StepSyntaxList : StepSyntax
    {
        public override StepSyntaxType SyntaxType => StepSyntaxType.List;

        public List<StepSyntax> Values { get; }

        public StepSyntaxList(int line, int column, IEnumerable<StepSyntax> values)
            : base(line, column)
        {
            Values = values.ToList();
        }
    }
}
