namespace IxMilia.Step.Tokens
{
    internal class StepRealToken : StepToken
    {
        public override StepTokenKind Kind => StepTokenKind.Real;

        public double Value { get; }

        public StepRealToken(double value, int line, int column)
            : base(line, column)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value.ToString("0.0#");
        }
    }
}
