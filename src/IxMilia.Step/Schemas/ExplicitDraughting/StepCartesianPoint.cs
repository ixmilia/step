using IxMilia.Step.Extensions;

namespace IxMilia.Step.Schemas.ExplicitDraughting
{
    public partial class StepCartesianPoint
    {
        public double X
        {
            get => Coordinates.GetValueAtIndexOrDefault(0);
            set => Coordinates.SetValueAtIndexAndEnsureCount(0, value);
        }

        public double Y
        {
            get => Coordinates.GetValueAtIndexOrDefault(1);
            set => Coordinates.SetValueAtIndexAndEnsureCount(1, value);
        }

        public double Z
        {
            get => Coordinates.GetValueAtIndexOrDefault(2);
            set => Coordinates.SetValueAtIndexAndEnsureCount(2, value);
        }

        public StepCartesianPoint(string name, double x, double y, double z)
            : this(name, new[] { x, y, z })
        {
        }
    }
}
