namespace IxMilia.Step.Tokens
{
    internal class StepStringToken : StepToken
    {
        public override StepTokenKind Kind => StepTokenKind.String;

        public string Value { get; }

        public StepStringToken(string value, int line, int column)
            : base(line, column)
        {
            Value = value;
        }

        public override string ToString()
        {
            // TODO: escaping
            return "'" + Value + "'";
        }
    }
}
