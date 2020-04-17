using IxMilia.Step.Syntax;

namespace IxMilia.Step.Items
{
    public class StepPlane : StepElementarySurface
    {
        public override StepItemType ItemType => StepItemType.Plane;

        private StepPlane()
            : base()
        {
        }

        public StepPlane(string name, StepAxis2Placement3D position)
            : base(name, position)
        {
        }

        internal static StepPlane CreateFromSyntaxList(StepBinder binder, StepSyntaxList syntaxList)
        {
            var plane = new StepPlane();
            syntaxList.AssertListCount(2);
            plane.Name = syntaxList.Values[0].GetStringValue();
            binder.BindValue(syntaxList.Values[1], v => plane.Position = v.AsType<StepAxis2Placement3D>());
            return plane;
        }
    }
}
