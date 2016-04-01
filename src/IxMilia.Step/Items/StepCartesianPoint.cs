// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using IxMilia.Step.Syntax;

namespace IxMilia.Step.Items
{
    public class StepCartesianPoint : StepTriple
    {
        public override StepItemType ItemType => StepItemType.CartesianPoint;
        protected override int MinimumValueCount => 1;

        private StepCartesianPoint()
        {
        }

        public StepCartesianPoint(string label, double x, double y, double z)
            : base(label, x, y, z)
        {
        }

        internal static StepCartesianPoint CreateFromSyntaxList(StepSyntaxList syntaxList)
        {
            return (StepCartesianPoint)AssignTo(new StepCartesianPoint(), syntaxList);
        }
    }
}
