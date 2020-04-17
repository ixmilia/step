using System.IO;
using System.Linq;
using IxMilia.Step.Items;
using Xunit;

namespace IxMilia.Step.Test
{
    public class StepFileTests : StepTestBase
    {
        [Fact]
        public void FileSystemAPITest()
        {
            var filePath = Path.GetTempFileName();
            var stepFile = new StepFile();
            var point = new StepCartesianPoint("some-label", 1.0, 2.0, 3.0);
            stepFile.Items.Add(point);

            // round trip
            stepFile.Save(filePath);
            var stepFile2 = StepFile.Load(filePath);

            var point2 = (StepCartesianPoint)stepFile2.Items.Single();
            Assert.Equal(point.Name, point2.Name);
            Assert.Equal(point.X, point2.X);
            Assert.Equal(point.Y, point2.Y);
            Assert.Equal(point.Z, point2.Z);

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
