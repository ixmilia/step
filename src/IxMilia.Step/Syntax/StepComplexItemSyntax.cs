// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;

namespace IxMilia.Step.Syntax
{
    internal class StepComplexItemSyntax : StepItemSyntax
    {
        public override StepSyntaxType SyntaxType => StepSyntaxType.ComplexItem;

        public List<StepSimpleItemSyntax> Items { get; } = new List<StepSimpleItemSyntax>();

        public StepComplexItemSyntax(int line, int column, IEnumerable<StepSimpleItemSyntax> items)
            : base(line, column)
        {
            Items = items.ToList();
        }

        public override string ToString(StepWriter writer)
        {
            return string.Join(string.Empty, Items.Select(e => e.ToString(writer)));
        }
    }
}
