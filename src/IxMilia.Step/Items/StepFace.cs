using System.Collections.Generic;
using System.Linq;
using IxMilia.Step.Syntax;

namespace IxMilia.Step.Items
{
    public abstract class StepFace : StepTopologicalRepresentationItem
    {
        public List<StepFaceBound> Bounds { get; } = new List<StepFaceBound>();

        public StepFace(string name)
            : base(name)
        {
        }

        internal override IEnumerable<StepSyntax> GetParameters(StepWriter writer)
        {
            foreach (var parameter in base.GetParameters(writer))
            {
                yield return parameter;
            }

            yield return new StepSyntaxList(Bounds.Select(b => writer.GetItemSyntax(b)));
        }
    }
}
