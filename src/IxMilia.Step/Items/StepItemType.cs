// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;

namespace IxMilia.Step.Items
{
    public enum StepItemType
    {
        AxisPlacement2D,
        CartesianPoint,
        Circle,
        Direction,
        Ellipse,
        Line,
        Vector
    }

    internal static class StepItemTypeExtensions
    {
        public const string AxisPlacement2DText = "AXIS2_PLACEMENT_2D";
        public const string CartesianPointText = "CARTESIAN_POINT";
        public const string CircleText = "CIRCLE";
        public const string DirectionText = "DIRECTION";
        public const string EllipseText = "ELLIPSE";
        public const string LineText = "LINE";
        public const string VectorText = "VECTOR";

        public static string GetItemTypeString(this StepItemType type)
        {
            switch (type)
            {
                case StepItemType.AxisPlacement2D:
                    return AxisPlacement2DText;
                case StepItemType.CartesianPoint:
                    return CartesianPointText;
                case StepItemType.Circle:
                    return CircleText;
                case StepItemType.Direction:
                    return DirectionText;
                case StepItemType.Ellipse:
                    return EllipseText;
                case StepItemType.Line:
                    return LineText;
                case StepItemType.Vector:
                    return VectorText;
                default:
                    throw new InvalidOperationException("Unexpected item type " + type);
            }
        }
    }
}
