using NUnit.Framework;

namespace HarinezumiSama.Benchmarks.StringFormatting.Tests
{
    [TestFixture]
    internal sealed class StringFormattingUsingStringValuesBenchmarksTests
        : StringFormattingBenchmarksTestsBase<StringFormattingUsingValuesBenchmarks>
    {
        [Test]
        public void TestResultValuesMatchEachOther()
        {
            var testee = CreateTestee();

            var expectedValue = testee.ConcatenateValues();
            Assert.That(() => testee.ConcatenateValuesInSuboptimalWay(), Is.EqualTo(expectedValue));
            Assert.That(() => testee.FormatValuesUsingStringFormatMethod(), Is.EqualTo(expectedValue));
            Assert.That(() => testee.FormatValuesUsingStringInterpolation(), Is.EqualTo(expectedValue));
            Assert.That(() => testee.FormatValuesUsingStringBuilderClass(), Is.EqualTo(expectedValue));
        }
    }
}