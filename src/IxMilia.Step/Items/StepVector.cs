using System;
using System.Collections.Generic;
using IxMilia.Step.Syntax;

namespace IxMilia.Step.Items
{
    public class StepVector : StepGeometricRepresentationItem
    {
        public override StepItemType ItemType => StepItemType.Vector;

        private StepDirection _direction;
        public StepDirection Direction
        {
            get { return _direction; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                _direction = value;
            }
        }
        public double Length { get; set; }

        private StepVector()
            : base(string.Empty)
        {
        }

        public StepVector(string name, StepDirection direction, double length)
            : base(name)
        {
            Direction = direction;
            Length = length;
        }

        internal override IEnumerable<StepRepresentationItem> GetReferencedItems()
        {
            yield return Direction;
        }

        internal override IEnumerable<StepSyntax> GetParameters(StepWriter writer)
        {
            foreach (var parameter in base.GetParameters(writer))
            {
                yield return parameter;
            }

            yield return writer.GetItemSyntax(Direction);
            yield return new StepRealSyntax(Length);
        }

        internal static StepVector CreateFromSyntaxList(StepBinder binder, StepSyntaxList syntaxList)
        {
            var vector = new StepVector();
            syntaxList.AssertListCount(3);
            vector.Name = syntaxList.Values[0].GetStringValue();
            binder.BindValue(syntaxList.Values[1], v => vector.Direction = v.AsType<StepDirection>());
            vector.Length = syntaxList.Values[2].GetRealVavlue();
            return vector;
        }
    }
}
