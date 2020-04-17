namespace IxMilia.Step.Tokens
{
    internal class StepEnumerationToken : StepToken
    {
        public override StepTokenKind Kind => StepTokenKind.Enumeration;

        public string Value { get; }

        public StepEnumerationToken(string value, int line, int column)
            : base(line, column)
        {
            Value = value;
        }

        public override string ToString()
        {
            return "." + Value + ".";
        }
    }
}
