using System.Collections.Generic;
using IxMilia.Step.Syntax;

namespace IxMilia.Step.Items
{
    public class StepFaceBound : StepTopologicalRepresentationItem
    {
        public override StepItemType ItemType => StepItemType.FaceBound;

        public StepLoop Bound { get; set; }
        public bool Orientation { get; set; }

        protected StepFaceBound()
            : base(string.Empty)
        {
        }

        public StepFaceBound(string name, StepLoop bound, bool orientation)
            : base(name)
        {
            Bound = bound;
            Orientation = orientation;
        }

        internal override IEnumerable<StepRepresentationItem> GetReferencedItems()
        {
            yield return Bound;
        }

        internal override IEnumerable<StepSyntax> GetParameters(StepWriter writer)
        {
            foreach (var parameter in base.GetParameters(writer))
            {
                yield return parameter;
            }

            yield return writer.GetItemSyntax(Bound);
            yield return StepWriter.GetBooleanSyntax(Orientation);
        }

        internal static StepFaceBound CreateFromSyntaxList(StepBinder binder, StepSyntaxList syntaxList)
        {
            syntaxList.AssertListCount(3);
            var faceBound = new StepFaceBound();
            faceBound.Name = syntaxList.Values[0].GetStringValue();
            binder.BindValue(syntaxList.Values[1], v => faceBound.Bound = v.AsType<StepLoop>());
            faceBound.Orientation = syntaxList.Values[2].GetBooleanValue();
            return faceBound;
        }
    }
}
