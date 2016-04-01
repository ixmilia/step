// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using IxMilia.Step.Syntax;

namespace IxMilia.Step.Entities
{
    public class StepAxisPlacement2D : StepEntity
    {
        public override StepEntityType EntityType => StepEntityType.AxisPlacement2D;

        private StepCartesianPoint _location;
        private StepDirection _direction;

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

        public StepDirection Direction
        {
            get { return _direction; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                _direction = value;
            }
        }

        private StepAxisPlacement2D()
            : base(string.Empty)
        {
        }

        public StepAxisPlacement2D(string label, StepCartesianPoint location, StepDirection direction)
            : base(label)
        {
            Location = location;
            Direction = direction;
        }

        internal override IEnumerable<StepEntity> GetReferencedEntities()
        {
            yield return Location;
            yield return Direction;
        }

        internal override IEnumerable<StepSyntax> GetParameters(StepWriter writer)
        {
            yield return new StepStringSyntax(Label);
            yield return writer.GetEntitySyntax(Location);
            yield return writer.GetEntitySyntax(Direction);
        }

        internal static StepAxisPlacement2D CreateFromSyntaxList(StepBinder binder, StepSyntaxList syntaxList)
        {
            var axis = new StepAxisPlacement2D();
            syntaxList.AssertListCount(3);
            axis.Label = syntaxList.Values[0].GetStringValue();
            binder.BindValue(syntaxList.Values[1], v => axis.Location = v.AsType<StepCartesianPoint>());
            binder.BindValue(syntaxList.Values[2], v => axis.Direction = v.AsType<StepDirection>());
            return axis;
        }
    }
}
