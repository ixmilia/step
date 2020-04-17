using System;
using System.Collections.Generic;
using IxMilia.Step.Syntax;

namespace IxMilia.Step.Items
{
    public class StepVertexPoint : StepVertex
    {
        public override StepItemType ItemType => StepItemType.VertexPoint;

        private StepCartesianPoint _location;

        public StepCartesianPoint Location
        {
            get { return _location; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                _location = value;
            }
        }

        private StepVertexPoint()
            : base(string.Empty)
        {
        }

        public StepVertexPoint(string name, StepCartesianPoint location)
            : base(name)
        {
            Location = location;
        }

        internal override IEnumerable<StepRepresentationItem> GetReferencedItems()
        {
            yield return Location;
        }

        internal override IEnumerable<StepSyntax> GetParameters(StepWriter writer)
        {
            foreach (var parameter in base.GetParameters(writer))
            {
                yield return parameter;
            }

            yield return writer.GetItemSyntax(Location);
        }

        internal static StepVertexPoint CreateFromSyntaxList(StepBinder binder, StepSyntaxList syntaxList)
        {
            var vertex = new StepVertexPoint();
            syntaxList.AssertListCount(2);
            vertex.Name = syntaxList.Values[0].GetStringValue();
            binder.BindValue(syntaxList.Values[1], v => vertex.Location = v.AsType<StepCartesianPoint>());
            return vertex;
        }
    }
}
