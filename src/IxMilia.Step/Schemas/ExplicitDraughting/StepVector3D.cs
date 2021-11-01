using System;

namespace IxMilia.Step.Schemas.ExplicitDraughting
{
    public struct StepVector3D
    {
        public double X { get; }
        public double Y { get; }
        public double Z { get; }

        public StepVector3D(double x, double y, double z)
            : this()
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double LengthSquared => X * X + Y * Y + Z * Z;

        public double Length => Math.Sqrt(LengthSquared);

        public bool IsZeroVector => X == 0.0 && Y == 0.0 && Z == 0.0;

        public StepVector3D Normalize() => this / Length;

        public StepVector3D Cross(StepVector3D v) => new StepVector3D(Y * v.Z - Z * v.Y, Z * v.X - X * v.Z, X * v.Y - Y * v.X);

        public double Dot(StepVector3D v) => X * v.X + Y * v.Y + Z * v.Z;

        public static StepVector3D operator -(StepVector3D vector) => new StepVector3D(-vector.X, -vector.Y, -vector.Z);

        public static StepVector3D operator +(StepVector3D p1, StepVector3D p2) => new StepVector3D(p1.X + p2.X, p1.Y + p2.Y, p1.Z + p2.Z);

        public static StepVector3D operator -(StepVector3D p1, StepVector3D p2) => new StepVector3D(p1.X - p2.X, p1.Y - p2.Y, p1.Z - p2.Z);

        public static StepVector3D operator *(StepVector3D vector, double operand) => new StepVector3D(vector.X * operand, vector.Y * operand, vector.Z * operand);

        public static StepVector3D operator /(StepVector3D vector, double operand) => new StepVector3D(vector.X / operand, vector.Y / operand, vector.Z / operand);

        public static bool operator ==(StepVector3D p1, StepVector3D p2) => p1.X == p2.X && p1.Y == p2.Y && p1.Z == p2.Z;

        public static bool operator !=(StepVector3D p1, StepVector3D p2) => !(p1 == p2);

        public override bool Equals(object obj) => obj is StepVector3D && this == (StepVector3D)obj;

        public bool Equals(StepVector3D other) => this == other;

        public override int GetHashCode() => X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode();

        public bool IsParallelTo(StepVector3D other) => Cross(other).IsZeroVector;

        public static StepVector3D XAxis => new StepVector3D(1.0, 0.0, 0.0);

        public static StepVector3D YAxis => new StepVector3D(0.0, 1.0, 0.0);

        public static StepVector3D ZAxis => new StepVector3D(0.0, 0.0, 1.0);

        public static StepVector3D One => new StepVector3D(1.0, 1.0, 1.0);

        public static StepVector3D Zero => new StepVector3D(0.0, 0.0, 0.0);

        public static StepVector3D SixtyDegrees => new StepVector3D(0.5, Math.Sqrt(3.0) * 0.5, 0);

        public override string ToString() => string.Format("({0},{1},{2})", X, Y, Z);

        public static StepVector3D RightVectorFromNormal(StepVector3D normal)
        {
            if (normal == XAxis)
            {
                return ZAxis;
            }

            var right = XAxis;
            var up = normal.Cross(right);
            return up.Cross(normal).Normalize();
        }

        public static StepVector3D NormalFromRightVector(StepVector3D right)
        {
            // these two functions are identical, but the separate name makes them easier to understand
            return RightVectorFromNormal(right);
        }
    }
}
