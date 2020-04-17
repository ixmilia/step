using System.Collections.Generic;
using IxMilia.Step.Syntax;

namespace IxMilia.Step.Items
{
    public abstract class StepTriple : StepPoint
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

        internal override IEnumerable<StepSyntax> GetParameters(StepWriter writer)
        {
            foreach (var parameter in base.GetParameters(writer))
            {
                yield return parameter;
            }

            yield return new StepSyntaxList(
                new StepRealSyntax(X),
                new StepRealSyntax(Y),
                new StepRealSyntax(Z)
            );
        }

        internal static StepTriple AssignTo(StepTriple triple, StepSyntaxList values)
        {
            values.AssertListCount(2);
            triple.Name = values.Values[0].GetStringValue();
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

            return ItemType == other.ItemType && X == other.X && Y == other.Y && Z == other.Z && Name == other.Name;
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
