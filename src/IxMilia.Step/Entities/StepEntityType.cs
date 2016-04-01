// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;

namespace IxMilia.Step.Entities
{
    public enum StepEntityType
    {
        AxisPlacement2D,
        CartesianPoint,
        Circle,
        Direction,
        Line,
        Vector
    }

    internal static class StepEntityTypeExtensions
    {
        public const string AxisPlacement2DText = "AXIS2_PLACEMENT_2D";
        public const string CartesianPointText = "CARTESIAN_POINT";
        public const string CircleText = "CIRCLE";
        public const string DirectionText = "DIRECTION";
        public const string LineText = "LINE";
        public const string VectorText = "VECTOR";

        public static string GetEntityTypeString(this StepEntityType type)
        {
            switch (type)
            {
                case StepEntityType.AxisPlacement2D:
                    return AxisPlacement2DText;
                case StepEntityType.CartesianPoint:
                    return CartesianPointText;
                case StepEntityType.Circle:
                    return CircleText;
                case StepEntityType.Direction:
                    return DirectionText;
                case StepEntityType.Line:
                    return LineText;
                case StepEntityType.Vector:
                    return VectorText;
                default:
                    throw new InvalidOperationException("Unexpected entity type " + type);
            }
        }
    }
}
