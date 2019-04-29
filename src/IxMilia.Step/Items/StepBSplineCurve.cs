// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using IxMilia.Step.Syntax;

namespace IxMilia.Step.Items
{
    public enum BSplineCurveForm
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

        public List<StepCartesianPoint> ControlPointsList { get; private set; }

        public BSplineCurveForm CurveForm { get; set; } = BSplineCurveForm.Unspecified;

        public bool ClosedCurve { get; set; }

        public bool SelfIntersect { get; set; }

        public StepBSplineCurve( string name, IEnumerable<StepCartesianPoint> controlPoints ) : base( name )
        {
            ControlPointsList = new List<StepCartesianPoint>( controlPoints );
        }

        public StepBSplineCurve( string name, params StepCartesianPoint[] controlPoints )
            : this( name, (IEnumerable<StepCartesianPoint>)controlPoints )
        {
        }

        internal override IEnumerable<StepRepresentationItem> GetReferencedItems()
        {
            return ControlPointsList;
        }

        internal override IEnumerable<StepSyntax> GetParameters( StepWriter writer )
        {
            foreach (var parameter in base.GetParameters( writer ) )
            {
                yield return parameter;
            }

            yield return new StepIntegerSyntax( Degree );
            yield return new StepSyntaxList( ControlPointsList.Select( c => writer.GetItemSyntax( c ) ) );
            yield return new StepEnumerationValueSyntax( GetCurveFormString( CurveForm ) );
            yield return new StepEnumerationValueSyntax( ClosedCurve ? "T" : "F" );
            yield return new StepEnumerationValueSyntax( SelfIntersect ? "T" : "F" );
        }

        private const string POLYLINE_FORM = "POLYLINE_FORM";
        private const string CIRCULAR_ARC = "CIRCULAR_ARC";
        private const string ELLIPTIC_ARC = "ELLIPTIC_ARC";
        private const string PARABOLIC_ARC = "PARABOLIC_ARC";
        private const string HYPERBOLIC_ARC = "HYPERBOLIC_ARC";
        private const string UNSPECIFIED = "UNSPECIFIED";


        protected static BSplineCurveForm ParseCurveForm( string enumerationValue )
        {
            switch ( enumerationValue.ToUpperInvariant() )
            {
                case POLYLINE_FORM:
                    return BSplineCurveForm.Polyline;
                case CIRCULAR_ARC:
                    return BSplineCurveForm.CircularArc;
                case ELLIPTIC_ARC:
                    return BSplineCurveForm.EllipticalArc;
                case PARABOLIC_ARC:
                    return BSplineCurveForm.ParabolicArc;
                case HYPERBOLIC_ARC:
                    return BSplineCurveForm.HyperbolicArc;
                default:
                    return BSplineCurveForm.Unspecified;
            }
        }

        protected static string GetCurveFormString( BSplineCurveForm form )
        {
            switch ( form )
            {
                case BSplineCurveForm.Polyline:
                    return POLYLINE_FORM;
                case BSplineCurveForm.CircularArc:
                    return CIRCULAR_ARC;
                case BSplineCurveForm.EllipticalArc:
                    return ELLIPTIC_ARC;
                case BSplineCurveForm.ParabolicArc:
                    return PARABOLIC_ARC;
                case BSplineCurveForm.HyperbolicArc:
                    return HYPERBOLIC_ARC;
                case BSplineCurveForm.Unspecified:
                    return UNSPECIFIED;
            }

            throw new NotImplementedException();
        }
    }
}
