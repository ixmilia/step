using System;
using System.Collections.Generic;
using IxMilia.Step.Syntax;

namespace IxMilia.Step.Items
{
    public class StepEdgeCurve : StepEdge
    {
        public override StepItemType ItemType => StepItemType.EdgeCurve;

        private StepCurve _edgeGeometry;

        public StepCurve EdgeGeometry
        {
            get { return _edgeGeometry; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                _edgeGeometry = value;
            }
        }

        public bool IsSameSense { get; set; }

        private StepEdgeCurve()
            : base()
        {
        }

        public StepEdgeCurve(string name, StepVertex edgeStart, StepVertex edgeEnd, StepCurve edgeGeometry, bool isSameSense)
            : base(name, edgeStart, edgeEnd)
        {
            EdgeGeometry = edgeGeometry;
            IsSameSense = isSameSense;
        }

        internal override IEnumerable<StepRepresentationItem> GetReferencedItems()
        {
            foreach (var item in base.GetReferencedItems())
            {
                yield return item;
            }

            yield return EdgeGeometry;
        }

        internal override IEnumerable<StepSyntax> GetParameters(StepWriter writer)
        {
            foreach (var parameter in base.GetParameters(writer))
            {
                yield return parameter;
            }

            yield return writer.GetItemSyntax(EdgeGeometry);
            yield return StepWriter.GetBooleanSyntax(IsSameSense);
        }

        internal static StepEdgeCurve CreateFromSyntaxList(StepBinder binder, StepSyntaxList syntaxList)
        {
            var edgeCurve = new StepEdgeCurve();
            syntaxList.AssertListCount(5);
            edgeCurve.Name = syntaxList.Values[0].GetStringValue();
            binder.BindValue(syntaxList.Values[1], v => edgeCurve.EdgeStart = v.AsType<StepVertex>());
            binder.BindValue(syntaxList.Values[2], v => edgeCurve.EdgeEnd = v.AsType<StepVertex>());
            binder.BindValue(syntaxList.Values[3], v => edgeCurve.EdgeGeometry = v.AsType<StepCurve>());
            edgeCurve.IsSameSense = syntaxList.Values[4].GetBooleanValue();
            return edgeCurve;
        }
    }
}
