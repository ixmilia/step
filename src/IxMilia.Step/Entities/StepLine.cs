// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using IxMilia.Step.Syntax;

namespace IxMilia.Step.Entities
{
    public class StepLine : StepEntity
    {
        public override StepEntityType EntityType => StepEntityType.Line;

        private StepCartesianPoint _point;
        private StepVector _vector;

        public StepCartesianPoint Point
        {
            get { return _point; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                _point = value;
            }
        }

        public StepVector Vector
        {
            get { return _vector; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                _vector = value;
            }
        }

        private StepLine()
            : base(string.Empty)
        {
        }

        public StepLine(string label, StepCartesianPoint point, StepVector vector)
            : base(label)
        {
            Point = point;
            Vector = vector;
        }

        internal override IEnumerable<StepEntity> GetReferencedEntities()
        {
            yield return Point;
            yield return Vector;
        }

        internal override IEnumerable<StepSyntax> GetParameters(StepWriter writer)
        {
            yield return new StepStringSyntax(Label);
            yield return writer.GetEntitySyntax(Point);
            yield return writer.GetEntitySyntax(Vector);
        }

        internal static StepLine CreateFromSyntaxList(StepBinder binder, StepSyntaxList syntaxList)
        {
            var line = new StepLine();
            syntaxList.AssertListCount(3);
            line.Label = syntaxList.Values[0].GetStringValue();
            binder.BindValue(syntaxList.Values[1], v => line.Point = v.AsType<StepCartesianPoint>());
            binder.BindValue(syntaxList.Values[2], v => line.Vector = v.AsType<StepVector>());
            return line;
        }
    }
}
