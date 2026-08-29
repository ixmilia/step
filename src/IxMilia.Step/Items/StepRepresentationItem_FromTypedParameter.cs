using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
                item = CreateFromSimpleItem(binder, simpleItem);
            }
            else if (itemSyntax is StepComplexItemSyntax)
            {
                // Complex entity instance - multiple types combined (e.g., BOUNDED_CURVE + B_SPLINE_CURVE + ...)
                var complexItem = (StepComplexItemSyntax)itemSyntax;
                item = CreateFromComplexItem(binder, complexItem);
            }

            return item;
        }

        private static StepRepresentationItem CreateFromSimpleItem(StepBinder binder, StepSimpleItemSyntax simpleItem)
        {
            StepRepresentationItem item = null;
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
                case StepItemTypeExtensions.ClosedShellText:
                    item = StepClosedShell.CreateFromSyntaxList(binder, simpleItem.Parameters);
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
                case StepItemTypeExtensions.ManifoldSolidBrepText:
                    item = StepManifoldSolidBrep.CreateFromSyntaxList(binder, simpleItem.Parameters);
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

            return item;
        }

        private static StepRepresentationItem CreateFromComplexItem(StepBinder binder, StepComplexItemSyntax complexItem)
        {
            // Complex entity instances combine multiple types. We need to find one we can handle.
            // For B-spline curves, the structure is typically:
            // (BOUNDED_CURVE() B_SPLINE_CURVE(...) B_SPLINE_CURVE_WITH_KNOTS(...) RATIONAL_B_SPLINE_CURVE(...) ...)
            // 
            // For B-spline surfaces, the structure is typically:
            // (BOUNDED_SURFACE() B_SPLINE_SURFACE(...) B_SPLINE_SURFACE_WITH_KNOTS(...) RATIONAL_B_SPLINE_SURFACE(...) ...)

            StepSimpleItemSyntax bSplineCurveItem = null;
            StepSimpleItemSyntax bSplineCurveWithKnotsItem = null;
            StepSimpleItemSyntax representationItem = null;
            bool isSurface = false;
            bool isCurve = false;

            foreach (var subItem in complexItem.Items)
            {
                switch (subItem.Keyword)
                {
                    case "B_SPLINE_CURVE":
                        bSplineCurveItem = subItem;
                        isCurve = true;
                        break;
                    case StepItemTypeExtensions.BSplineCurveWithKnotsText:
                        bSplineCurveWithKnotsItem = subItem;
                        isCurve = true;
                        break;
                    case "RATIONAL_B_SPLINE_CURVE":
                        isCurve = true;
                        break;
                    case "BOUNDED_CURVE":
                        isCurve = true;
                        break;
                    case "REPRESENTATION_ITEM":
                        representationItem = subItem;
                        break;
                    case "B_SPLINE_SURFACE":
                    case "B_SPLINE_SURFACE_WITH_KNOTS":
                    case "RATIONAL_B_SPLINE_SURFACE":
                    case "BOUNDED_SURFACE":
                        isSurface = true;
                        break;
                }
            }

            // Try to create a B_SPLINE_CURVE_WITH_KNOTS from the combined parameters
            if (bSplineCurveItem != null && bSplineCurveWithKnotsItem != null)
            {
                return CreateBSplineCurveWithKnotsFromComplex(binder, bSplineCurveItem, bSplineCurveWithKnotsItem, representationItem);
            }

            // For complex surface entities, return a placeholder so they can be referenced
            if (isSurface)
            {
                var keywords = string.Join("+", complexItem.Items.Select(i => i.Keyword));
                string name = string.Empty;
                if (representationItem != null && representationItem.Parameters.Values.Count > 0)
                {
                    name = representationItem.Parameters.Values[0].GetStringValue();
                }
                return new StepUnsupportedSurface(name, keywords);
            }

            // Fallback: try each sub-item to see if any can be parsed directly
            foreach (var subItem in complexItem.Items)
            {
                var item = CreateFromSimpleItem(binder, subItem);
                if (item != null)
                {
                    return item;
                }
            }

            // Log unsupported complex entity
            var allKeywords = string.Join("+", complexItem.Items.Select(i => i.Keyword));
            if (UnsupportedItemTypes.Add($"COMPLEX({allKeywords})"))
            {
                Debug.WriteLine($"Unsupported complex item ({allKeywords}) at {complexItem.Line}, {complexItem.Column}");
            }

            return null;
        }

        private static StepBSplineCurveWithKnots CreateBSplineCurveWithKnotsFromComplex(
            StepBinder binder,
            StepSimpleItemSyntax bSplineCurveItem,
            StepSimpleItemSyntax bSplineCurveWithKnotsItem,
            StepSimpleItemSyntax representationItem)
        {
            // B_SPLINE_CURVE parameters (indices 0-4):
            // 0: degree (integer)
            // 1: control_points_list (list of cartesian_point references)
            // 2: curve_form (enumeration)
            // 3: closed_curve (boolean)
            // 4: self_intersect (boolean)
            var bSplineParams = bSplineCurveItem.Parameters;

            // B_SPLINE_CURVE_WITH_KNOTS additional parameters (indices 0-2):
            // 0: knot_multiplicities (list of integers)
            // 1: knots (list of reals)
            // 2: knot_spec (enumeration)
            var knotsParams = bSplineCurveWithKnotsItem.Parameters;

            // Get name from REPRESENTATION_ITEM if available, otherwise empty
            string name = string.Empty;
            if (representationItem != null && representationItem.Parameters.Values.Count > 0)
            {
                name = representationItem.Parameters.Values[0].GetStringValue();
            }

            // Merge into a combined syntax list matching the expected format for StepBSplineCurveWithKnots.CreateFromSyntaxList
            // Expected format (9 parameters):
            // 0: name
            // 1: degree
            // 2: control_points_list
            // 3: curve_form
            // 4: closed_curve
            // 5: self_intersect
            // 6: knot_multiplicities
            // 7: knots
            // 8: knot_spec

            var combinedValues = new List<StepSyntax>
            {
                new StepStringSyntax(name),
                bSplineParams.Values[0], // degree
                bSplineParams.Values[1], // control_points_list
                bSplineParams.Values[2], // curve_form
                bSplineParams.Values[3], // closed_curve
                bSplineParams.Values[4], // self_intersect
                knotsParams.Values[0],   // knot_multiplicities
                knotsParams.Values[1],   // knots
                knotsParams.Values[2]    // knot_spec
            };

            var combinedSyntaxList = new StepSyntaxList(bSplineCurveItem.Line, bSplineCurveItem.Column, combinedValues);
            return StepBSplineCurveWithKnots.CreateFromSyntaxList(binder, combinedSyntaxList);
        }
    }
}
