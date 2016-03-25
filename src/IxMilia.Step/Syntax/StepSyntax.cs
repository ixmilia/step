// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace IxMilia.Step.Syntax
{
    internal abstract class StepSyntax
    {
        public abstract StepSyntaxType SyntaxType { get; }

        public int Line { get; }
        public int Column { get; }

        protected StepSyntax(int line, int column)
        {
            Line = line;
            Column = column;
        }
    }
}
