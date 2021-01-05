using NUnit.Framework;

namespace HarinezumiSama.Benchmarks.StringFormatting.Tests
{
    [TestFixture]
    internal abstract class StringFormattingBenchmarksTestsBase<T>
        where T : StringFormattingBenchmarks, new()
    {
        protected static T CreateTestee() => new();
    }
}