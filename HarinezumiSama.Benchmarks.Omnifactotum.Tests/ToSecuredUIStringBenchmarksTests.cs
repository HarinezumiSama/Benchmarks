using System.Linq;
using HarinezumiSama.Benchmarks.Omnifactotum.StringExtensions;
using NUnit.Framework;

namespace HarinezumiSama.Benchmarks.Omnifactotum.Tests;

[TestFixture(TypeArgs = [typeof(ToSecuredUIStringEmptyStringValueBenchmarks)])]
[TestFixture(TypeArgs = [typeof(ToSecuredUIStringSingleCharValueBenchmarks)])]
[TestFixture(TypeArgs = [typeof(ToSecuredUIStringExtraShortValueBenchmarks)])]
[TestFixture(TypeArgs = [typeof(ToSecuredUIStringShortValueBenchmarks)])]
[TestFixture(TypeArgs = [typeof(ToSecuredUIStringLongValueBenchmarks)])]
[TestFixture(TypeArgs = [typeof(ToSecuredUIStringExtraLongValueBenchmarks)])]
[TestFixture(TypeArgs = [typeof(ToSecuredUIStringHugeValueBenchmarks)])]
public sealed class ToSecuredUIStringBenchmarksTests<TBenchmark>
    where TBenchmark : ToSecuredUIStringBenchmarksBase, new()
{
    [Test]
    public void TestResultValuesMatchEachOther()
    {
        var testee = CreateTestee();

        foreach (var value in testee.InputValueValues.Cast<string?>().Prepend(null))
        {
            testee.InputValue = value;

            var expectedValue = testee.F0_Initial();
            Assert.That(() => testee.F1_New(), Is.EqualTo(expectedValue));
        }
    }

    private static TBenchmark CreateTestee() => new();
}