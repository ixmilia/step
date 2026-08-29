namespace IxMilia.Step.Items
{
    /// <summary>
    /// A placeholder for surface types that are not fully supported.
    /// This allows parsing to continue when encountering complex surface entities.
    /// </summary>
    public class StepUnsupportedSurface : StepSurface
    {
        public string OriginalType { get; }

        public StepUnsupportedSurface(string name, string originalType)
            : base(name)
        {
            OriginalType = originalType;
        }

        public override StepItemType ItemType => StepItemType.Plane; // Use Plane as a stand-in for type checking
    }
}
