namespace IxMilia.Step.Tokens
{
    internal class StepOmittedToken : StepToken
    {
        public override StepTokenKind Kind => StepTokenKind.Omitted;

        public StepOmittedToken(int line, int column)
            : base(line, column)
        {
        }

        public override string ToString()
        {
            return "$";
        }

        public static StepOmittedToken Instance { get; } = new StepOmittedToken(-1, -1);
    }
}
