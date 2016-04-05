// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using IxMilia.Step.Syntax;

namespace IxMilia.Step.Items
{
    public abstract partial class StepRepresentationItem
    {
        internal static StepRepresentationItem FromTypedParameter(StepBinder binder, StepItemSyntax itemSyntax)
        {
            StepRepresentationItem item = null;
            if (itemSyntax is StepSimpleItemSyntax)
            {
                var simpleitem = (StepSimpleItemSyntax)itemSyntax;
                switch (simpleitem.Keyword)
                {
                    case StepItemTypeExtensions.Axis2Placement2DText:
                        item = StepAxis2Placement2D.CreateFromSyntaxList(binder, simpleitem.Parameters);
                        break;
                    case StepItemTypeExtensions.Axis2Placement3DText:
                        item = StepAxis2Placement3D.CreateFromSyntaxList(binder, simpleitem.Parameters);
                        break;
                    case StepItemTypeExtensions.CartesianPointText:
                        item = StepCartesianPoint.CreateFromSyntaxList(simpleitem.Parameters);
                        break;
                    case StepItemTypeExtensions.CircleText:
                        item = StepCircle.CreateFromSyntaxList(binder, simpleitem.Parameters);
                        break;
                    case StepItemTypeExtensions.DirectionText:
                        item = StepDirection.CreateFromSyntaxList(simpleitem.Parameters);
                        break;
                    case StepItemTypeExtensions.EdgeCurveText:
                        item = StepEdgeCurve.CreateFromSyntaxList(binder, simpleitem.Parameters);
                        break;
                    case StepItemTypeExtensions.EllipseText:
                        item = StepEllipse.CreateFromSyntaxList(binder, simpleitem.Parameters);
                        break;
                    case StepItemTypeExtensions.LineText:
                        item = StepLine.CreateFromSyntaxList(binder, simpleitem.Parameters);
                        break;
                    case StepItemTypeExtensions.VectorText:
                        item = StepVector.CreateFromSyntaxList(binder, simpleitem.Parameters);
                        break;
                    case StepItemTypeExtensions.VertexPointText:
                        item = StepVertexPoint.CreateFromSyntaxList(binder, simpleitem.Parameters);
                        break;
                }
            }
            else
            {
                // TODO:
            }

            return item;
        }
    }
}
