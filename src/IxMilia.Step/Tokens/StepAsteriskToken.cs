namespace IxMilia.Step.Tokens
{
    internal class StepAsteriskToken : StepToken
    {
        public override StepTokenKind Kind => StepTokenKind.Asterisk;

        public StepAsteriskToken(int line, int column)
            : base(line, column)
        {
        }

        public override string ToString()
        {
            return "*";
        }

        public static StepAsteriskToken Instance { get; } = new StepAsteriskToken(-1, -1);
    }
}
