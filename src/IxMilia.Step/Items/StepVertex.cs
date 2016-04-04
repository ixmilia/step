// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using IxMilia.Step.Syntax;

namespace IxMilia.Step.Items
{
    public class StepVertex : StepTopologicalRepresentationItem
    {
        public override StepItemType ItemType => StepItemType.Vertex;

        private StepCartesianPoint _location;

        public StepCartesianPoint Location
        {
            get { return _location; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                _location = value;
            }
        }

        private StepVertex()
            : base(string.Empty)
        {
        }

        public StepVertex(string name, StepCartesianPoint location)
            : base(name)
        {
            Location = location;
        }

        internal override IEnumerable<StepRepresentationItem> GetReferencedItems()
        {
            yield return Location;
        }

        internal override IEnumerable<StepSyntax> GetParameters(StepWriter writer)
        {
            foreach (var parameter in base.GetParameters(writer))
            {
                yield return parameter;
            }

            yield return writer.GetItemSyntax(Location);
        }

        internal static StepVertex CreateFromSyntaxList(StepBinder binder, StepSyntaxList syntaxList)
        {
            var vertex = new StepVertex();
            syntaxList.AssertListCount(2);
            vertex.Name = syntaxList.Values[0].GetStringValue();
            binder.BindValue(syntaxList.Values[1], v => vertex.Location = v.AsType<StepCartesianPoint>());
            return vertex;
        }
    }
}
