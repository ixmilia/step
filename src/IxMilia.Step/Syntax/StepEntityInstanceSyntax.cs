using System.Collections.Generic;
using IxMilia.Step.Tokens;

namespace IxMilia.Step.Syntax
{
    internal class StepEntityInstanceSyntax : StepSyntax
    {
        public override StepSyntaxType SyntaxType => StepSyntaxType.EntityInstance;

        public int Id { get; }
        public StepItemSyntax SimpleItemInstance { get; }

        public StepEntityInstanceSyntax(StepEntityInstanceToken instanceId, StepItemSyntax itemInstance)
            : base(instanceId.Line, instanceId.Column)
        {
            Id = instanceId.Id;
            SimpleItemInstance = itemInstance;
        }

        public override IEnumerable<StepToken> GetTokens()
        {
            yield return new StepEntityInstanceToken(Id, -1, -1);
            foreach (var token in SimpleItemInstance.GetTokens())
            {
                yield return token;
            }
        }
    }
}
