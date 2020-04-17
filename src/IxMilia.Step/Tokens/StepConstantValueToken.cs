namespace IxMilia.Step.Tokens
{
    internal class StepConstantValueToken : StepToken
    {
        public override StepTokenKind Kind => StepTokenKind.ConstantValue;

        public string Name { get; }

        public StepConstantValueToken(string name, int line, int column)
            : base(line, column)
        {
            Name = name;
        }

        public override string ToString()
        {
            return "@" + Name;
        }
    }
}
