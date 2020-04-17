namespace IxMilia.Step.Tokens
{
    internal class StepCommaToken : StepToken
    {
        public override StepTokenKind Kind => StepTokenKind.Comma;

        public StepCommaToken(int line, int column)
            : base(line, column)
        {
        }

        public override string ToString()
        {
            return ",";
        }

        public static StepCommaToken Instance { get; } = new StepCommaToken(-1, -1);
    }
}
