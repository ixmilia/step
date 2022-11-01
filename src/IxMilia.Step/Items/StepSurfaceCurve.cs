using IxMilia.Step.Syntax;

namespace IxMilia.Step.Items
{
    public class StepSurfaceCurve : StepCurve
    {
        private StepSurfaceCurve(string name)
            : base(name)
        {
        }

        private StepSurfaceCurve()
        : base(string.Empty)
        {
        }

        public StepCurve Geometry;

        public override StepItemType ItemType => StepItemType.SurfaceCurve;

        internal static StepSurfaceCurve CreateFromSyntaxList(StepBinder binder, StepSyntaxList syntaxList)
        {            
            var curve = new StepSurfaceCurve();       
            curve.Name = syntaxList.Values[0].GetStringValue();

            binder.BindValue(syntaxList.Values[1], v => curve.Geometry = v.AsType<StepCurve>());
            
            return curve;
        }
    }
}
