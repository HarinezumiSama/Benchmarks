using System.Linq;
using HarinezumiSama.Benchmarks.Omnifactotum.StringExtensions;
using NUnit.Framework;

namespace HarinezumiSama.Benchmarks.Omnifactotum.Tests;

[TestFixture(TypeArgs = [typeof(ToUIStringEmptyStringValueBenchmarks)])]
[TestFixture(TypeArgs = [typeof(ToUIStringSingleCharValueBenchmarks)])]
[TestFixture(TypeArgs = [typeof(ToUIStringExtraShortValueBenchmarks)])]
[TestFixture(TypeArgs = [typeof(ToUIStringShortValueBenchmarks)])]
[TestFixture(TypeArgs = [typeof(ToUIStringLongValueBenchmarks)])]
[TestFixture(TypeArgs = [typeof(ToUIStringExtraLongValueBenchmarks)])]
[TestFixture(TypeArgs = [typeof(ToUIStringHugeValueBenchmarks)])]
public sealed class ToUIStringBenchmarksTests<TBenchmark>
    where TBenchmark : ToUIStringBenchmarksBase, new()
{
    [Test]
    public void TestResultValuesMatchEachOther()
    {
        var testee = CreateTestee();

        foreach (var value in testee.InputValueValues.Cast<string?>().Prepend(null))
        {
            testee.InputValue = value;

            var expectedValue = testee.F0_StringReplaceAndConcat();
            Assert.That(() => testee.F1_UsingStackOrHeapAllocationAndOnePreSearch(), Is.EqualTo(expectedValue));
        }
    }

    private static TBenchmark CreateTestee() => new();
}