using System.Collections.Generic;
using IxMilia.Step.Syntax;

namespace IxMilia.Step.Items
{
    public class StepAxis2Placement2D : StepAxis2Placement
    {
        public override StepItemType ItemType => StepItemType.AxisPlacement2D;

        private StepAxis2Placement2D()
            : base(string.Empty)
        {
        }

        public StepAxis2Placement2D(string label, StepCartesianPoint location, StepDirection direction)
            : base(label)
        {
            Location = location;
            RefDirection = direction;
        }

        internal override IEnumerable<StepRepresentationItem> GetReferencedItems()
        {
            yield return Location;
            yield return RefDirection;
        }

        internal override IEnumerable<StepSyntax> GetParameters(StepWriter writer)
        {
            foreach (var parameter in base.GetParameters(writer))
            {
                yield return parameter;
            }

            yield return writer.GetItemSyntax(Location);
            yield return writer.GetItemSyntax(RefDirection);
        }

        internal static StepAxis2Placement2D CreateFromSyntaxList(StepBinder binder, StepSyntaxList syntaxList)
        {
            var axis = new StepAxis2Placement2D();
            syntaxList.AssertListCount(3);
            axis.Name = syntaxList.Values[0].GetStringValue();
            binder.BindValue(syntaxList.Values[1], v => axis.Location = v.AsType<StepCartesianPoint>());
            binder.BindValue(syntaxList.Values[2], v => axis.RefDirection = v.AsType<StepDirection>());
            return axis;
        }
    }
}
