using System.Collections.Generic;
using IxMilia.Step.Syntax;

namespace IxMilia.Step.Items
{
    public abstract class StepEdge : StepTopologicalRepresentationItem
    {
        public StepVertex EdgeStart { get; set; }
        public StepVertex EdgeEnd { get; set; }

        protected StepEdge()
            : base(string.Empty)
        {
        }

        protected StepEdge(string name, StepVertex edgeStart, StepVertex edgeEnd)
            : base(name)
        {
            EdgeStart = edgeStart;
            EdgeEnd = edgeEnd;
        }

        internal override IEnumerable<StepRepresentationItem> GetReferencedItems()
        {
            if (EdgeStart != null)
            {
                yield return EdgeStart;
            }

            if (EdgeEnd != null)
            {
                yield return EdgeEnd;
            }
        }

        internal override IEnumerable<StepSyntax> GetParameters(StepWriter writer)
        {
            foreach (var parameter in base.GetParameters(writer))
            {
                yield return parameter;
            }

            yield return writer.GetItemSyntaxOrAuto(EdgeStart);
            yield return writer.GetItemSyntaxOrAuto(EdgeEnd);
        }
    }
}
