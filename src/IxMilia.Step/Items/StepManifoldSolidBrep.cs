using System.Collections.Generic;
using IxMilia.Step.Syntax;

namespace IxMilia.Step.Items
{
    public class StepManifoldSolidBrep : StepRepresentationItem
    {
        public override StepItemType ItemType => StepItemType.ManifoldSolidBrep;

        public StepClosedShell Shell { get; set; }

        public StepManifoldSolidBrep(string name)
            : base(name)
        {
        }

        private StepManifoldSolidBrep()
            : base(string.Empty)
        {
        }

        internal override IEnumerable<StepRepresentationItem> GetReferencedItems()
        {
            if (Shell != null)
                yield return Shell;
        }

        internal override IEnumerable<StepSyntax> GetParameters(StepWriter writer)
        {
            foreach (var parameter in base.GetParameters(writer))
            {
                yield return parameter;
            }

            yield return writer.GetItemSyntax(Shell);
        }

        internal static StepManifoldSolidBrep CreateFromSyntaxList(StepBinder binder, StepSyntaxList syntaxList)
        {
            var solid = new StepManifoldSolidBrep();
            syntaxList.AssertListCount(2);
            solid.Name = syntaxList.Values[0].GetStringValue();
            binder.BindValue(syntaxList.Values[1], v => solid.Shell = v.AsType<StepClosedShell>());
            return solid;
        }
    }
}
