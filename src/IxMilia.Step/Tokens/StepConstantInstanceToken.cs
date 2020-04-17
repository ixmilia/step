namespace IxMilia.Step.Tokens
{
    internal class StepConstantInstanceToken : StepToken
    {
        public override StepTokenKind Kind => StepTokenKind.ConstantInstance;

        public string Name { get; }

        public StepConstantInstanceToken(string name, int line, int column)
            : base(line, column)
        {
            Name = name;
        }

        public override string ToString()
        {
            return "#" + Name;
        }
    }
}
