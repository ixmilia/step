// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;

namespace IxMilia.Step
{
    public class StepValueList : StepValue
    {
        public List<StepValue> Values { get; }

        public StepValueList(IEnumerable<StepValue> values, int line, int column)
            : base(line, column)
        {
            Values = values.ToList();
        }
    }
}
