namespace IxMilia.Step.Tokens
{
    internal class StepRightParenToken : StepToken
    {
        public override StepTokenKind Kind => StepTokenKind.RightParen;

        public StepRightParenToken(int line, int column)
            : base(line, column)
        {
        }

        public override string ToString()
        {
            return ")";
        }

        public static StepRightParenToken Instance { get; } = new StepRightParenToken(-1, -1);
    }
}
