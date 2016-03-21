// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IxMilia.Step.Tokens;

namespace IxMilia.Step
{
    internal class StepReader
    {
        private StepTokenizer _tokenizer;

        public StepReader(Stream stream)
        {
            _tokenizer = new StepTokenizer(stream);
        }

        public StepFile ReadFile()
        {
            return null;
        }
    }
}
