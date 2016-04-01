// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;

namespace IxMilia.Step.Syntax
{
    internal class StepComplexEntitySyntax : StepEntitySyntax
    {
        public override StepSyntaxType SyntaxType => StepSyntaxType.ComplexEntity;

        public List<StepSimpleEntitySyntax> Entities { get; } = new List<StepSimpleEntitySyntax>();

        public StepComplexEntitySyntax(int line, int column, IEnumerable<StepSimpleEntitySyntax> entities)
            : base(line, column)
        {
            Entities = entities.ToList();
        }

        public override string ToString(StepWriter writer)
        {
            return string.Join(string.Empty, Entities.Select(e => e.ToString(writer)));
        }
    }
}
