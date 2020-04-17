using System.Collections.Generic;
using IxMilia.Step.Syntax;

namespace IxMilia.Step.Items
{
    public class StepCylindricalSurface : StepElementarySurface
    {
        public override StepItemType ItemType => StepItemType.CylindricalSurface;

        public double Radius { get; set; }

        private StepCylindricalSurface()
            : base()
        {
        }

        public StepCylindricalSurface(string name, StepAxis2Placement3D position, double radius)
            : base(name, position)
        {
            Radius = radius;
        }

        internal override IEnumerable<StepSyntax> GetParameters(StepWriter writer)
        {
            foreach (var parameter in base.GetParameters(writer))
            {
                yield return parameter;
            }

            yield return new StepRealSyntax(Radius);
        }

        internal static StepRepresentationItem CreateFromSyntaxList(StepBinder binder, StepSyntaxList syntaxList)
        {
            syntaxList.AssertListCount(3);
            var surface = new StepCylindricalSurface();
            surface.Name = syntaxList.Values[0].GetStringValue();
            binder.BindValue(syntaxList.Values[1], v => surface.Position = v.AsType<StepAxis2Placement3D>());
            surface.Radius = syntaxList.Values[2].GetRealVavlue();
            return surface;
        }
    }
}
