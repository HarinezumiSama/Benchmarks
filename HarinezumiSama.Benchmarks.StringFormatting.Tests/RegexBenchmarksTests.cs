#if NET7_0_OR_GREATER
using NUnit.Framework;

namespace HarinezumiSama.Benchmarks.StringFormatting.Tests;

[TestFixture]
internal sealed class RegexBenchmarksTests
{
    public static string[] InputValueValues => RegexBenchmarks.GetInputValues();

    [Test]
    public void TestResultValuesMatchEachOther([ValueSource(nameof(InputValueValues))] string inputValue)
    {
        var testee = CreateTestee(inputValue);

        var expectedValue = testee.SplitUsingDefaultGeneratedRegex();
        Assert.That(() => testee.SplitUsingNewGeneratedRegex(), Is.EqualTo(expectedValue));
    }

    private static RegexBenchmarks CreateTestee(string inputValue) => new() { InputValue = inputValue };
}
#endif