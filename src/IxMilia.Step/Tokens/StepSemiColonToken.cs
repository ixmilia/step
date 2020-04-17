namespace IxMilia.Step.Tokens
{
    internal class StepSemicolonToken : StepToken
    {
        public override StepTokenKind Kind => StepTokenKind.Semicolon;

        public StepSemicolonToken(int line, int column)
            : base(line, column)
        {
        }

        public override string ToString()
        {
            return ";";
        }

        public static StepSemicolonToken Instance { get; } = new StepSemicolonToken(-1, -1);
    }
}
