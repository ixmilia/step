namespace IxMilia.Step.Tokens
{
    internal class StepInstanceValueToken : StepToken
    {
        public override StepTokenKind Kind => StepTokenKind.InstanceValue;

        public int Id { get; }

        public StepInstanceValueToken(int id, int line, int column)
            : base(line, column)
        {
            Id = id;
        }

        public override string ToString()
        {
            return "@" + Id.ToString();
        }
    }
}
