using System;
using System.Collections.Generic;
using IxMilia.Step.Syntax;

namespace IxMilia.Step.Items
{
    public abstract class StepElementarySurface : StepSurface
    {
        private StepAxis2Placement3D _position;

        public StepAxis2Placement3D Position
        {
            get { return _position; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                _position = value;
            }
        }

        protected StepElementarySurface()
            : base(string.Empty)
        {
        }

        public StepElementarySurface(string name, StepAxis2Placement3D position)
            : base(name)
        {
            Position = position;
        }

        internal override IEnumerable<StepRepresentationItem> GetReferencedItems()
        {
            yield return Position;
        }

        internal override IEnumerable<StepSyntax> GetParameters(StepWriter writer)
        {
            foreach (var parameter in base.GetParameters(writer))
            {
                yield return parameter;
            }

            yield return writer.GetItemSyntax(Position);
        }
    }
}
