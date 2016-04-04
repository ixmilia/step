// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using IxMilia.Step.Syntax;

namespace IxMilia.Step.Items
{
    public abstract class StepEdge : StepTopologicalRepresentationItem
    {
        private StepVertex _edgeStart;
        private StepVertex _edgeEnd;

        public StepVertex EdgeStart
        {
            get { return _edgeStart; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                _edgeStart = value;
            }
        }

        public StepVertex EdgeEnd
        {
            get { return _edgeEnd; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                _edgeEnd = value;
            }
        }

        protected StepEdge()
            : base(string.Empty)
        {
        }

        protected StepEdge(string name, StepVertex edgeStart, StepVertex edgeEnd)
            : base(name)
        {
            EdgeStart = edgeStart;
            EdgeEnd = edgeEnd;
        }

        internal override IEnumerable<StepRepresentationItem> GetReferencedItems()
        {
            yield return EdgeStart;
            yield return EdgeEnd;
        }

        internal override IEnumerable<StepSyntax> GetParameters(StepWriter writer)
        {
            foreach (var parameter in base.GetParameters(writer))
            {
                yield return parameter;
            }

            yield return writer.GetItemSyntax(EdgeStart);
            yield return writer.GetItemSyntax(EdgeEnd);
        }
    }
}
