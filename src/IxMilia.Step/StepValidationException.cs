using System;

namespace IxMilia.Step
{
    public class StepValidationException : Exception
    {
        public StepValidationException(string message)
            : base(message)
        {
        }
    }
}
