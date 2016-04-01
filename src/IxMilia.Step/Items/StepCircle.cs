// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using IxMilia.Step.Syntax;

namespace IxMilia.Step.Items
{
    public class StepCircle : StepConic
    {
        public override StepItemType ItemType => StepItemType.Circle;

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

        internal override IEnumerable<StepRepresentationItem> GetReferencedItems()
        {
            yield return Position;
        }

        internal override IEnumerable<StepSyntax> GetParameters(StepWriter writer)
        {
            foreach (var parameter in base.GetParameters(writer))
            {
                yield return parameter;
            }

            yield return writer.GetItemSyntax(Position);
            yield return new StepRealSyntax(Radius);
        }

        internal static StepCircle CreateFromSyntaxList(StepBinder binder, StepSyntaxList syntaxList)
        {
            var circle = new StepCircle();
            syntaxList.AssertListCount(3);
            circle.Name = syntaxList.Values[0].GetStringValue();
            binder.BindValue(syntaxList.Values[1], v => circle.Position = v.AsType<StepAxisPlacement2D>());
            circle.Radius = syntaxList.Values[2].GetRealVavlue();
            return circle;
        }
    }
}
