using System.Linq;
using Xunit;

namespace IxMilia.Step.Test
{
    public abstract class StepTestBase
    {
        protected static string NormalizeLineEndings(string str)
        {
            var lines = str.Split('\n').Select(l => l.TrimEnd('\r'));
            return string.Join("\r\n", lines);
        }

        protected void AssertFileIs(StepFile file, string expected, bool inlineReferences = false)
        {
            var actual = file.GetContentsAsString(inlineReferences);
            var expectedNormalizedLines = NormalizeLineEndings(expected);
            Assert.Equal(expectedNormalizedLines, actual);
        }

        protected void AssertFileContains(StepFile file, string expected, bool inlineReferences = false)
        {
            var actual = file.GetContentsAsString(inlineReferences);
            var expectedNormalizedLines = NormalizeLineEndings(expected);
            Assert.Contains(expectedNormalizedLines, actual);
        }
    }
}
