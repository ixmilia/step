using System.Collections.Generic;
using System.Diagnostics;
using IxMilia.Step.Syntax;

namespace IxMilia.Step.Items
{
    public abstract partial class StepRepresentationItem
    {
        internal static HashSet<string> UnsupportedItemTypes { get; } = new HashSet<string>();

        internal static StepRepresentationItem FromTypedParameter(StepBinder binder, StepItemSyntax itemSyntax)
        {
            StepRepresentationItem item = null;
            if (itemSyntax is StepSimpleItemSyntax)
            {
                var simpleItem = (StepSimpleItemSyntax)itemSyntax;
                switch (simpleItem.Keyword)
                {
                    case StepItemTypeExtensions.AdvancedFaceText:
                        item = StepAdvancedFace.CreateFromSyntaxList(binder, simpleItem.Parameters);
                        break;
                    case StepItemTypeExtensions.Axis2Placement2DText:
                        item = StepAxis2Placement2D.CreateFromSyntaxList(binder, simpleItem.Parameters);
                        break;
                    case StepItemTypeExtensions.Axis2Placement3DText:
                        item = StepAxis2Placement3D.CreateFromSyntaxList(binder, simpleItem.Parameters);
                        break;
                    case StepItemTypeExtensions.BSplineCurveWithKnotsText:
                        item = StepBSplineCurveWithKnots.CreateFromSyntaxList(binder, simpleItem.Parameters);
                        break;
                    case StepItemTypeExtensions.CartesianPointText:
                        item = StepCartesianPoint.CreateFromSyntaxList(simpleItem.Parameters);
                        break;
                    case StepItemTypeExtensions.CircleText:
                        item = StepCircle.CreateFromSyntaxList(binder, simpleItem.Parameters);
                        break;
                    case StepItemTypeExtensions.CylindricalSurfaceText:
                        item = StepCylindricalSurface.CreateFromSyntaxList(binder, simpleItem.Parameters);
                        break;
                    case StepItemTypeExtensions.DirectionText:
                        item = StepDirection.CreateFromSyntaxList(simpleItem.Parameters);
                        break;
                    case StepItemTypeExtensions.EdgeCurveText:
                        item = StepEdgeCurve.CreateFromSyntaxList(binder, simpleItem.Parameters);
                        break;
                    case StepItemTypeExtensions.EdgeLoopText:
                        item = StepEdgeLoop.CreateFromSyntaxList(binder, simpleItem.Parameters);
                        break;
                    case StepItemTypeExtensions.EllipseText:
                        item = StepEllipse.CreateFromSyntaxList(binder, simpleItem.Parameters);
                        break;
                    case StepItemTypeExtensions.FaceBoundText:
                        item = StepFaceBound.CreateFromSyntaxList(binder, simpleItem.Parameters);
                        break;
                    case StepItemTypeExtensions.FaceOuterBoundText:
                        item = StepFaceOuterBound.CreateFromSyntaxList(binder, simpleItem.Parameters);
                        break;
                    case StepItemTypeExtensions.LineText:
                        item = StepLine.CreateFromSyntaxList(binder, simpleItem.Parameters);
                        break;
                    case StepItemTypeExtensions.OrientedEdgeText:
                        item = StepOrientedEdge.CreateFromSyntaxList(binder, simpleItem.Parameters);
                        break;
                    case StepItemTypeExtensions.PlaneText:
                        item = StepPlane.CreateFromSyntaxList(binder, simpleItem.Parameters);
                        break;
                    case StepItemTypeExtensions.VectorText:
                        item = StepVector.CreateFromSyntaxList(binder, simpleItem.Parameters);
                        break;
                    case StepItemTypeExtensions.VertexPointText:
                        item = StepVertexPoint.CreateFromSyntaxList(binder, simpleItem.Parameters);
                        break;
                    default:
                        if (UnsupportedItemTypes.Add(simpleItem.Keyword))
                        {
                            Debug.WriteLine($"Unsupported item {simpleItem.Keyword} at {simpleItem.Line}, {simpleItem.Column}");
                        }
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
