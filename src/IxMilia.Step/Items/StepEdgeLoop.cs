using System.Collections.Generic;
using System.Linq;
using IxMilia.Step.Syntax;

namespace IxMilia.Step.Items
{
    public class StepEdgeLoop : StepLoop
    {
        public override StepItemType ItemType => StepItemType.EdgeLoop;

        public List<StepOrientedEdge> EdgeList { get; private set; }

        public StepEdgeLoop(string name, IEnumerable<StepOrientedEdge> edgeList)
            : base(name)
        {
            EdgeList = new List<StepOrientedEdge>(edgeList);
        }

        public StepEdgeLoop(string name, params StepOrientedEdge[] edgeList)
            : this(name, (IEnumerable<StepOrientedEdge>)edgeList)
        {
        }

        internal override IEnumerable<StepRepresentationItem> GetReferencedItems()
        {
            return EdgeList;
        }

        internal override IEnumerable<StepSyntax> GetParameters(StepWriter writer)
        {
            foreach (var parameter in base.GetParameters(writer))
            {
                yield return parameter;
            }

            yield return new StepSyntaxList(-1, -1, EdgeList.Select(e => writer.GetItemSyntax(e)));
        }

        internal static StepEdgeLoop CreateFromSyntaxList(StepBinder binder, StepSyntaxList syntaxList)
        {
            syntaxList.AssertListCount(2);
            var edgeSyntaxList = syntaxList.Values[1].GetValueList();
            var edgeLoop = new StepEdgeLoop(string.Empty, new StepOrientedEdge[edgeSyntaxList.Values.Count]);
            edgeLoop.Name = syntaxList.Values[0].GetStringValue();
            for (int i = 0; i < edgeSyntaxList.Values.Count; i++)
            {
                var j = i; // capture to avoid rebinding
                binder.BindValue(edgeSyntaxList.Values[j], v => edgeLoop.EdgeList[j] = v.AsType<StepOrientedEdge>());
            }

            return edgeLoop;
        }
    }
}
