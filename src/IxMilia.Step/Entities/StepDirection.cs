// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

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

        internal static StepDirection CreateFromMacro(StepMacro macro)
        {
            return (StepDirection)AssignTo(new StepDirection(), macro);
        }
    }
}
