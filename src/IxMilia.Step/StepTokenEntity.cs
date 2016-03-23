// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace IxMilia.Step
{
    internal class StepTokenEntity
    {
        public int Id { get; }
        public StepMacro Macro { get; }

        public StepTokenEntity(int id, StepMacro macro)
        {
            Id = id;
            Macro = macro;
        }
    }
}
