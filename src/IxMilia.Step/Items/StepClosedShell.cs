using System.Collections.Generic;
using System.Linq;
using IxMilia.Step.Syntax;

namespace IxMilia.Step.Items
{
    public class StepClosedShell : StepTopologicalRepresentationItem
    {
        public List<StepAdvancedFace> Faces { get; } = new List<StepAdvancedFace>();

        public StepClosedShell(string name)
            : base(name)
        {
        }

        private StepClosedShell()
            : base(string.Empty)
        {
        }

        public override StepItemType ItemType => StepItemType.ClosedShell;

        internal static StepClosedShell CreateFromSyntaxList(StepBinder binder, StepSyntaxList syntaxList)
        {
            var shell = new StepClosedShell();
            
            shell.Name = syntaxList.Values[0].GetStringValue();

            var boundsList = syntaxList.Values[1].GetValueList();
            shell.Faces.Clear();
            shell.Faces.AddRange(Enumerable.Range(0, boundsList.Values.Count).Select(_ => (StepAdvancedFace)null));
            for (int i = 0; i < boundsList.Values.Count; i++)
            {
                var j = i; // capture to avoid rebinding
                binder.BindValue(boundsList.Values[j], v => shell.Faces[j] = v.AsType<StepAdvancedFace>());
            }            

            return shell;
        }
    }
}
