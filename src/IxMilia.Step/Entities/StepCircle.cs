// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using IxMilia.Step.Syntax;

namespace IxMilia.Step.Entities
{
    public class StepCircle : StepEntity
    {
        public override StepEntityType EntityType => StepEntityType.Circle;

        private StepAxisPlacement2D _position;

        public StepAxisPlacement2D Position
        {
            get { return _position; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                _position = value;
            }
        }

        public double Radius { get; set; }

        private StepCircle()
            : base(string.Empty)
        {
        }

        public StepCircle(string label, StepAxisPlacement2D position, double radius)
            : base(label)
        {
            Position = position;
            Radius = radius;
        }

        internal override IEnumerable<StepEntity> GetReferencedEntities()
        {
            yield return Position;
        }

        internal override IEnumerable<StepSyntax> GetParameters(StepWriter writer)
        {
            yield return new StepStringSyntax(Label);
            yield return writer.GetEntitySyntax(Position);
            yield return new StepRealSyntax(Radius);
        }

        internal static StepCircle CreateFromSyntaxList(StepBinder binder, StepSyntaxList syntaxList)
        {
            var circle = new StepCircle();
            syntaxList.AssertListCount(3);
            circle.Label = syntaxList.Values[0].GetStringValue();
            binder.BindValue(syntaxList.Values[1], v => circle.Position = v.AsType<StepAxisPlacement2D>());
            circle.Radius = syntaxList.Values[2].GetRealVavlue();
            return circle;
        }
    }
}
