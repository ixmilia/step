// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using IxMilia.Step.Syntax;

namespace IxMilia.Step.Entities
{
    public abstract partial class StepEntity
    {
        public abstract StepEntityType EntityType { get; }

        public string Label { get; set; }

        protected StepEntity(string label)
        {
            Label = label;
        }

        internal virtual IEnumerable<StepEntity> GetReferencedEntities()
        {
            yield break;
        }

        internal abstract IEnumerable<StepSyntax> GetParameters(StepWriter writer);
    }
}
