using System.IO;
using System.Linq;
using IxMilia.Step.Schemas.ExplicitDraughting;
using Xunit;

namespace IxMilia.Step.Test
{
    public class StepFileTests : StepTestBase
    {
        [Fact]
        public void FileSystemAPITest()
        {
            string filePath = null;
            try
            {
                filePath = Path.GetTempFileName();
                var stepFile = new StepFile();
                var point = new StepCartesianPoint("some-label", new StepVector3D(1.0, 2.0, 3.0));
                stepFile.Items.Add(point);

                // round trip
                stepFile.Save(filePath);
                var stepFile2 = StepFile.Load(filePath);

                var point2 = (StepCartesianPoint)stepFile2.Items.Single();
                Assert.Equal(point.Name, point2.Name);
                Assert.Equal(point.Coordinates.X, point2.Coordinates.X);
                Assert.Equal(point.Coordinates.Y, point2.Coordinates.Y);
                Assert.Equal(point.Coordinates.Z, point2.Coordinates.Z);
            }
            finally
            {
                try
                {
                    File.Delete(filePath);
                }
                catch
                {
                }
            }
        }
    }
}
