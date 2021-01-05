using NUnit.Framework;

namespace HarinezumiSama.Benchmarks.StringFormatting.Tests
{
    [TestFixture]
    internal sealed class StringFormattingUsingStringsBenchmarksTests
        : StringFormattingBenchmarksTestsBase<StringFormattingUsingStringsBenchmarks>
    {
        [Test]
        public void TestResultValuesMatchEachOther()
        {
            var testee = CreateTestee();

            var expectedValue = testee.ConcatenateStrings();
            Assert.That(() => testee.ConcatenateStringsInSuboptimalWay(), Is.EqualTo(expectedValue));
            Assert.That(() => testee.FormatStringsUsingStringFormatMethod(), Is.EqualTo(expectedValue));
            Assert.That(() => testee.FormatStringsUsingStringInterpolation(), Is.EqualTo(expectedValue));
            Assert.That(() => testee.FormatStringsUsingStringBuilderClass(), Is.EqualTo(expectedValue));
        }
    }
}