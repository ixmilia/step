using System.Collections.Generic;
using System.Linq;
using IxMilia.Step.Syntax;

namespace IxMilia.Step.Items
{
    public class StepClosedShell : StepRepresentationItem
    {
        public override StepItemType ItemType => StepItemType.ClosedShell;

        public List<StepAdvancedFace> FaceList { get; set; } = new List<StepAdvancedFace>();

        public StepClosedShell(string name)
            : base(name)
        {
        }

        private StepClosedShell()
            : base(string.Empty)
        {
        }

        internal override IEnumerable<StepRepresentationItem> GetReferencedItems()
        {
            return FaceList.Cast<StepRepresentationItem>();
        }

        internal override IEnumerable<StepSyntax> GetParameters(StepWriter writer)
        {
            foreach (var parameter in base.GetParameters(writer))
            {
                yield return parameter;
            }

            yield return new StepSyntaxList(FaceList.Select(f => writer.GetItemSyntax(f)));
        }

        internal static StepClosedShell CreateFromSyntaxList(StepBinder binder, StepSyntaxList syntaxList)
        {
            var shell = new StepClosedShell();
            syntaxList.AssertListCount(2);
            shell.Name = syntaxList.Values[0].GetStringValue();
            
            var faceListSyntax = syntaxList.Values[1].GetValueList();
            foreach (var faceSyntax in faceListSyntax.Values)
            {
                binder.BindValue(faceSyntax, v =>
                {
                    var face = v.AsType<StepAdvancedFace>();
                    if (face != null)
                        shell.FaceList.Add(face);
                });
            }
            
            return shell;
        }
    }
}
