using System;
using System.Collections.Generic;
using IxMilia.Step.Syntax;

namespace IxMilia.Step.Items
{
    public class StepLine : StepCurve
    {
        public override StepItemType ItemType => StepItemType.Line;

        private StepCartesianPoint _point;
        private StepVector _vector;

        public StepCartesianPoint Point
        {
            get { return _point; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                _point = value;
            }
        }

        public StepVector Vector
        {
            get { return _vector; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                _vector = value;
            }
        }

        private StepLine()
            : base(string.Empty)
        {
        }

        public StepLine(string label, StepCartesianPoint point, StepVector vector)
            : base(label)
        {
            Point = point;
            Vector = vector;
        }

        public static StepLine FromPoints(double x1, double y1, double z1, double x2, double y2, double z2)
        {
            var start = new StepCartesianPoint("", x1, y1, z1);
            var dx = x2 - x1;
            var dy = y2 - y1;
            var dz = z2 - z1;
            var length = Math.Sqrt(dx * dx + dy * dy + dz * dz);
            var dxn = dx / length;
            var dyn = dy / length;
            var dzn = dz / length;
            var vector = new StepVector("", new StepDirection("", dxn, dyn, dzn), length);
            return new StepLine("", start, vector);
        }

        internal override IEnumerable<StepRepresentationItem> GetReferencedItems()
        {
            yield return Point;
            yield return Vector;
        }

        internal override IEnumerable<StepSyntax> GetParameters(StepWriter writer)
        {
            foreach (var parameter in base.GetParameters(writer))
            {
                yield return parameter;
            }

            yield return writer.GetItemSyntax(Point);
            yield return writer.GetItemSyntax(Vector);
        }

        internal static StepLine CreateFromSyntaxList(StepBinder binder, StepSyntaxList syntaxList)
        {
            var line = new StepLine();
            syntaxList.AssertListCount(3);
            line.Name = syntaxList.Values[0].GetStringValue();
            binder.BindValue(syntaxList.Values[1], v => line.Point = v.AsType<StepCartesianPoint>());
            binder.BindValue(syntaxList.Values[2], v => line.Vector = v.AsType<StepVector>());
            return line;
        }
    }
}
