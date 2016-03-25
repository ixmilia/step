// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using IxMilia.Step.Syntax;

namespace IxMilia.Step.Entities
{
    public class StepVector : StepEntity
    {
        public override StepEntityType EntityType => StepEntityType.Vector;

        private StepDirection _direction;
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
        public double Length { get; set; }

        private StepVector()
            : base(string.Empty)
        {
        }

        public StepVector(string name, StepDirection direction, double length)
            : base(name)
        {
            Direction = direction;
            Length = length;
        }

        internal static StepVector CreateFromSyntaxList(StepBinder binder, StepSyntaxList syntaxList)
        {
            var vector = new StepVector();
            syntaxList.AssertListCount(3);
            vector.Name = syntaxList.Values[0].GetStringValue();
            binder.BindValue(syntaxList.Values[1], v => vector.Direction = v.AsType<StepDirection>());
            vector.Length = syntaxList.Values[2].GetRealVavlue();
            return vector;
        }
    }
}
