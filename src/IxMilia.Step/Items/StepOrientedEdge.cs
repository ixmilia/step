using System;
using System.Collections.Generic;
using IxMilia.Step.Syntax;

namespace IxMilia.Step.Items
{
    public class StepOrientedEdge : StepEdge
    {
        public override StepItemType ItemType => StepItemType.OrientedEdge;

        private StepEdge _edgeElement;

        public StepEdge EdgeElement
        {
            get { return _edgeElement; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                _edgeElement = value;
            }
        }

        public bool Orientation { get; set; }

        private StepOrientedEdge()
        {
        }

        public StepOrientedEdge(string name, StepVertex edgeStart, StepVertex edgeEnd, StepEdge edgeElement, bool orientation)
            : base(name, edgeStart, edgeEnd)
        {
            EdgeElement = edgeElement;
            Orientation = orientation;
        }

        internal override IEnumerable<StepRepresentationItem> GetReferencedItems()
        {
            foreach (var item in base.GetReferencedItems())
            {
                yield return item;
            }

            yield return EdgeElement;
        }

        internal override IEnumerable<StepSyntax> GetParameters(StepWriter writer)
        {
            foreach (var parameter in base.GetParameters(writer))
            {
                yield return parameter;
            }

            yield return writer.GetItemSyntax(EdgeElement);
            yield return StepWriter.GetBooleanSyntax(Orientation);
        }

        internal static StepOrientedEdge CreateFromSyntaxList(StepBinder binder, StepSyntaxList syntaxList)
        {
            var orientedEdge = new StepOrientedEdge();
            syntaxList.AssertListCount(5);
            orientedEdge.Name = syntaxList.Values[0].GetStringValue();
            binder.BindValue(syntaxList.Values[1], v => orientedEdge.EdgeStart = v.AsType<StepVertex>());
            binder.BindValue(syntaxList.Values[2], v => orientedEdge.EdgeEnd = v.AsType<StepVertex>());
            binder.BindValue(syntaxList.Values[3], v => orientedEdge.EdgeElement = v.AsType<StepEdge>());
            orientedEdge.Orientation = syntaxList.Values[4].GetBooleanValue();
            return orientedEdge;
        }
    }
}
