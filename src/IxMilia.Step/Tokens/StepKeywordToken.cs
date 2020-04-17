namespace IxMilia.Step.Tokens
{
    internal class StepKeywordToken : StepToken
    {
        public override StepTokenKind Kind => StepTokenKind.Keyword;

        public string Value { get; }

        public StepKeywordToken(string value, int line, int column)
            : base(line, column)
        {
            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
