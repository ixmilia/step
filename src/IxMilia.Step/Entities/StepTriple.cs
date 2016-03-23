// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace IxMilia.Step.Entities
{
    public abstract class StepTriple : StepEntity
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        protected abstract int MinimumValueCount { get; }

        protected StepTriple()
            : this(string.Empty, 0.0, 0.0, 0.0)
        {
        }

        protected StepTriple(string name, double x, double y, double z)
            : base(name)
        {
            X = x;
            Y = y;
            Z = z;
        }

        internal static StepTriple AssignTo(StepTriple triple, StepMacro macro)
        {
            macro.Values.AssertValueListCount(2);
            triple.Name = macro.Values.Values[0].GetStringValue();
            var pointValues = macro.Values.Values[1].GetValueList();
            pointValues.AssertValueListCount(triple.MinimumValueCount, 3);
            triple.X = pointValues.GetDoubleValueOrDefault(0);
            triple.Y = pointValues.GetDoubleValueOrDefault(1);
            triple.Z = pointValues.GetDoubleValueOrDefault(2);
            return triple;
        }

        public bool Equals(StepTriple other)
        {
            if ((object)other == null)
            {
                return false;
            }

            return EntityType == other.EntityType && X == other.X && Y == other.Y && Z == other.Z && Name == other.Name;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as StepTriple);
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();
        }

        public static bool operator ==(StepTriple left, StepTriple right)
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

        public static bool operator !=(StepTriple left, StepTriple right)
        {
            return !(left == right);
        }
    }
}
