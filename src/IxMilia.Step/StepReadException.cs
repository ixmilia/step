using System;

namespace IxMilia.Step
{
    public class StepReadException : Exception
    {
        public int Line { get; }
        public int Column { get; }

        public StepReadException(string message, int line, int column)
            : base($"{message} at [{line}:{column}]")
        {
            Line = line;
            Column = column;
        }
    }
}
