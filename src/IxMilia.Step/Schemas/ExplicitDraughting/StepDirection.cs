using IxMilia.Step.Extensions;

namespace IxMilia.Step.Schemas.ExplicitDraughting
{
    public partial class StepDirection
    {
        public double X
        {
            get => DirectionRatios.GetValueAtIndexOrDefault(0);
            set => DirectionRatios.SetValueAtIndexAndEnsureCount(0, value);
        }

        public double Y
        {
            get => DirectionRatios.GetValueAtIndexOrDefault(1);
            set => DirectionRatios.SetValueAtIndexAndEnsureCount(1, value);
        }

        public double Z
        {
            get => DirectionRatios.GetValueAtIndexOrDefault(2);
            set => DirectionRatios.SetValueAtIndexAndEnsureCount(2, value);
        }

        public StepDirection(string name, double x, double y, double z)
            : this(name, new[] { x, y, z })
        {
        }
    }
}
