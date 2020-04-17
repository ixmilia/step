using System;
using System.Collections.Generic;
using System.Linq;
using IxMilia.Step.Syntax;

namespace IxMilia.Step.Items
{
    public enum StepBSplineCurveForm
    {
        Polyline,
        CircularArc,
        EllipticalArc,
        ParabolicArc,
        HyperbolicArc,
        Unspecified
    };

    public abstract class StepBSplineCurve : StepBoundedCurve
    {
        public int Degree { get; set; }

        public List<StepCartesianPoint> ControlPointsList { get; } = new List<StepCartesianPoint>();

        public StepBSplineCurveForm CurveForm { get; set; } = StepBSplineCurveForm.Unspecified;

        public bool ClosedCurve { get; set; }

        public bool SelfIntersect { get; set; }

        public StepBSplineCurve(string name, IEnumerable<StepCartesianPoint> controlPoints) : base(name)
        {
            ControlPointsList.AddRange(controlPoints);
        }

        public StepBSplineCurve(string name, params StepCartesianPoint[] controlPoints)
            : this(name, (IEnumerable<StepCartesianPoint>)controlPoints)
        {
        }

        internal override IEnumerable<StepRepresentationItem> GetReferencedItems()
        {
            return ControlPointsList;
        }

        internal override IEnumerable<StepSyntax> GetParameters(StepWriter writer)
        {
            foreach (var parameter in base.GetParameters(writer))
            {
                yield return parameter;
            }

            yield return new StepIntegerSyntax(Degree);
            yield return new StepSyntaxList(ControlPointsList.Select(c => writer.GetItemSyntax(c)));
            yield return new StepEnumerationValueSyntax(GetCurveFormString(CurveForm));
            yield return StepWriter.GetBooleanSyntax(ClosedCurve);
            yield return StepWriter.GetBooleanSyntax(SelfIntersect);
        }

        private const string POLYLINE_FORM = "POLYLINE_FORM";
        private const string CIRCULAR_ARC = "CIRCULAR_ARC";
        private const string ELLIPTIC_ARC = "ELLIPTIC_ARC";
        private const string PARABOLIC_ARC = "PARABOLIC_ARC";
        private const string HYPERBOLIC_ARC = "HYPERBOLIC_ARC";
        private const string UNSPECIFIED = "UNSPECIFIED";

        protected static StepBSplineCurveForm ParseCurveForm(string enumerationValue)
        {
            switch (enumerationValue.ToUpperInvariant())
            {
                case POLYLINE_FORM:
                    return StepBSplineCurveForm.Polyline;
                case CIRCULAR_ARC:
                    return StepBSplineCurveForm.CircularArc;
                case ELLIPTIC_ARC:
                    return StepBSplineCurveForm.EllipticalArc;
                case PARABOLIC_ARC:
                    return StepBSplineCurveForm.ParabolicArc;
                case HYPERBOLIC_ARC:
                    return StepBSplineCurveForm.HyperbolicArc;
                default:
                    return StepBSplineCurveForm.Unspecified;
            }
        }

        protected static string GetCurveFormString(StepBSplineCurveForm form)
        {
            switch (form)
            {
                case StepBSplineCurveForm.Polyline:
                    return POLYLINE_FORM;
                case StepBSplineCurveForm.CircularArc:
                    return CIRCULAR_ARC;
                case StepBSplineCurveForm.EllipticalArc:
                    return ELLIPTIC_ARC;
                case StepBSplineCurveForm.ParabolicArc:
                    return PARABOLIC_ARC;
                case StepBSplineCurveForm.HyperbolicArc:
                    return HYPERBOLIC_ARC;
                case StepBSplineCurveForm.Unspecified:
                    return UNSPECIFIED;
            }

            throw new NotImplementedException();
        }
    }
}
