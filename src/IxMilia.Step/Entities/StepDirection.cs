// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using IxMilia.Step.Syntax;

namespace IxMilia.Step.Entities
{
    public class StepDirection : StepTriple
    {
        public override StepEntityType EntityType => StepEntityType.Direction;
        protected override int MinimumValueCount => 2;

        private StepDirection()
        {
        }

        public StepDirection(string name, double x, double y, double z)
            : base(name, x, y, z)
        {
        }

        internal static StepDirection CreateFromSyntaxList(StepSyntaxList syntaxList)
        {
            return (StepDirection)AssignTo(new StepDirection(), syntaxList);
        }
    }
}
