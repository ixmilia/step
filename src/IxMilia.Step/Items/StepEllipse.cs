// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using IxMilia.Step.Syntax;

namespace IxMilia.Step.Items
{
    public class StepEllipse : StepConic
    {
        public override StepItemType ItemType => StepItemType.Ellipse;

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

        public double SemiAxis1 { get; set; }
        public double SemiAxis2 { get; set; }

        private StepEllipse()
            : base(string.Empty)
        {
        }

        public StepEllipse(string name, StepAxisPlacement2D position, double semiAxis1, double semiAxis2)
            : base(name)
        {
            Position = position;
            SemiAxis1 = semiAxis1;
            SemiAxis2 = semiAxis2;
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
            yield return new StepRealSyntax(SemiAxis1);
            yield return new StepRealSyntax(SemiAxis2);
        }

        internal static StepEllipse CreateFromSyntaxList(StepBinder binder, StepSyntaxList syntaxList)
        {
            var ellipse = new StepEllipse();
            syntaxList.AssertListCount(4);
            ellipse.Name = syntaxList.Values[0].GetStringValue();
            binder.BindValue(syntaxList.Values[1], v => ellipse.Position = v.AsType<StepAxisPlacement2D>());
            ellipse.SemiAxis1 = syntaxList.Values[2].GetRealVavlue();
            ellipse.SemiAxis2 = syntaxList.Values[3].GetRealVavlue();
            return ellipse;
        }
    }
}
