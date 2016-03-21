// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

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
