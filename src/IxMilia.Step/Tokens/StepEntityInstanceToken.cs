namespace IxMilia.Step.Tokens
{
    internal class StepEntityInstanceToken : StepToken
    {
        public override StepTokenKind Kind => StepTokenKind.EntityInstance;

        public int Id { get; }

        public StepEntityInstanceToken(int id, int line, int column)
            : base(line, column)
        {
            Id = id;
        }

        public override string ToString()
        {
            return "#" + Id.ToString();
        }
    }
}
