namespace IxMilia.Step.Tokens
{
    internal abstract class StepToken
    {
        public abstract StepTokenKind Kind { get; }

        public int Line { get; }
        public int Column { get; }

        protected StepToken(int line, int column)
        {
            Line = line;
            Column = column;
        }

        public virtual string ToString(StepWriter writer)
        {
            return ToString();
        }
    }
}
