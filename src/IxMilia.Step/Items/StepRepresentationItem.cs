// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Generic;
using IxMilia.Step.Syntax;

namespace IxMilia.Step.Items
{
    public abstract partial class StepRepresentationItem
    {
        public abstract StepItemType ItemType { get; }

        public string Name { get; set; }

        protected StepRepresentationItem(string name)
        {
            Name = name;
        }

        internal virtual IEnumerable<StepRepresentationItem> GetReferencedItems()
        {
            yield break;
        }

        internal virtual IEnumerable<StepSyntax> GetParameters(StepWriter writer)
        {
            yield return new StepStringSyntax(Name);
        }
    }
}
