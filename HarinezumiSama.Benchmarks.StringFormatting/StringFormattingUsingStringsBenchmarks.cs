using System.Text;
using BenchmarkDotNet.Attributes;

namespace HarinezumiSama.Benchmarks.StringFormatting
{
    [InvocationCount(2_000_000)]
    public class StringFormattingUsingStringsBenchmarks : StringFormattingBenchmarks
    {
        [Benchmark(Baseline = true)]
        public string ConcatenateStrings()
            => "Timeout, ms: " + TimeoutAsString
                + ", URL: " + UrlAsString
                + ", Request ID: " + RequestIdAsString
                + ", Timestamp: " + TimestampAsString
                + ", Result: " + ResultAsString;

        [Benchmark]
        public string ConcatenateStringsInSuboptimalWay()
        {
            var result = "Timeout, ms: " + TimeoutAsString;

            result += ", URL: " + UrlAsString;
            result += ", Request ID: " + RequestIdAsString;
            result += ", Timestamp: " + TimestampAsString;
            result += ", Result: " + ResultAsString;

            return result;
        }

        [Benchmark]
        public string FormatStringsUsingStringFormatMethod()
            //// ReSharper disable once UseStringInterpolation
            => string.Format(
                @"Timeout, ms: {0}, URL: {1}, Request ID: {2}, Timestamp: {3}, Result: {4}",
                TimeoutAsString,
                UrlAsString,
                RequestIdAsString,
                TimestampAsString,
                ResultAsString);

        [Benchmark]
        public string FormatStringsUsingStringInterpolation()
            => $@"Timeout, ms: {TimeoutAsString:G}, URL: {UrlAsString:G}, Request ID: {RequestIdAsString:G}, Timestamp: {
                TimestampAsString:G}, Result: {ResultAsString:G}";

        [Benchmark]
        public string FormatStringsUsingStringBuilderClass()
            => new StringBuilder()
                .Append("Timeout, ms: ").Append(TimeoutAsString)
                .Append(", URL: ").Append(UrlAsString)
                .Append(", Request ID: ").Append(RequestIdAsString)
                .Append(", Timestamp: ").Append(TimestampAsString)
                .Append(", Result: ").Append(ResultAsString)
                .ToString();
    }
}