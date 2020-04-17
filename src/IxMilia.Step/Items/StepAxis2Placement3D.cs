using System;
using System.Collections.Generic;
using IxMilia.Step.Syntax;

namespace IxMilia.Step.Items
{
    public class StepAxis2Placement3D : StepAxis2Placement
    {
        public override StepItemType ItemType => StepItemType.AxisPlacement3D;

        private StepDirection _axis;

        public StepDirection Axis
        {
            get { return _axis; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                _axis = value;
            }
        }

        private StepAxis2Placement3D()
            : base(string.Empty)
        {
        }

        public StepAxis2Placement3D(string name, StepCartesianPoint location, StepDirection axis, StepDirection refDirection)
            : base(name)
        {
            Location = location;
            Axis = axis;
            RefDirection = refDirection;
        }

        internal override IEnumerable<StepRepresentationItem> GetReferencedItems()
        {
            yield return Location;
            yield return Axis;
            yield return RefDirection;
        }

        internal override IEnumerable<StepSyntax> GetParameters(StepWriter writer)
        {
            foreach (var parameter in base.GetParameters(writer))
            {
                yield return parameter;
            }

            yield return writer.GetItemSyntax(Location);
            yield return writer.GetItemSyntax(Axis);
            yield return writer.GetItemSyntax(RefDirection);
        }

        internal static StepAxis2Placement3D CreateFromSyntaxList(StepBinder binder, StepSyntaxList syntaxList)
        {
            var axis = new StepAxis2Placement3D();
            syntaxList.AssertListCount(4);
            axis.Name = syntaxList.Values[0].GetStringValue();
            binder.BindValue(syntaxList.Values[1], v => axis.Location = v.AsType<StepCartesianPoint>());
            binder.BindValue(syntaxList.Values[2], v => axis.Axis = v.AsType<StepDirection>());
            binder.BindValue(syntaxList.Values[3], v => axis.RefDirection = v.AsType<StepDirection>());
            return axis;
        }
    }
}
