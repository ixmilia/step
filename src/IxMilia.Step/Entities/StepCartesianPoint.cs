// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace IxMilia.Step.Entities
{
    public class StepCartesianPoint : StepEntity
    {
        public override StepEntityType EntityType => StepEntityType.CartesianPoint;

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        public StepCartesianPoint(string name, double x, double y, double z)
            : base(name)
        {
            X = x;
            Y = y;
            Z = z;
        }

        internal StepCartesianPoint(StepMacro macro)
            : base(string.Empty)
        {
            macro.Values.AssertValueListCount(2);
            Name = macro.Values.Values[0].GetStringValue();
            var pointValues = macro.Values.Values[1].GetValueList();
            pointValues.AssertValueListCount(1, 3);
            X = pointValues.GetDoubleValueOrDefault(0);
            Y = pointValues.GetDoubleValueOrDefault(1);
            Z = pointValues.GetDoubleValueOrDefault(2);
        }

        public bool Equals(StepCartesianPoint other)
        {
            if ((object)other == null)
            {
                return false;
            }

            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as StepCartesianPoint);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }

        public static bool operator ==(StepCartesianPoint left, StepCartesianPoint right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }

            if ((object)left == null || (object)right == null)
            {
                return false;
            }

            return left.Equals(right);
        }

        public static bool operator !=(StepCartesianPoint left, StepCartesianPoint right)
        {
            return !(left == right);
        }
    }
}
