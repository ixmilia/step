// Copyright (c) IxMilia.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using IxMilia.Step.Syntax;

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

        protected StepTriple(string label, double x, double y, double z)
            : base(label)
        {
            X = x;
            Y = y;
            Z = z;
        }

        internal static StepTriple AssignTo(StepTriple triple, StepSyntaxList values)
        {
            values.AssertListCount(2);
            triple.Label = values.Values[0].GetStringValue();
            var pointValues = values.Values[1].GetValueList();
            pointValues.AssertListCount(triple.MinimumValueCount, 3);
            triple.X = pointValues.GetRealValueOrDefault(0);
            triple.Y = pointValues.GetRealValueOrDefault(1);
            triple.Z = pointValues.GetRealValueOrDefault(2);
            return triple;
        }

        public bool Equals(StepTriple other)
        {
            if ((object)other == null)
            {
                return false;
            }

            return EntityType == other.EntityType && X == other.X && Y == other.Y && Z == other.Z && Label == other.Label;
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
