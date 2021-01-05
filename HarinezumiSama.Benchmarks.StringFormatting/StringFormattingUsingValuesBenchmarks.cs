using System.Text;
using BenchmarkDotNet.Attributes;

namespace HarinezumiSama.Benchmarks.StringFormatting
{
    [InvocationCount(250_000)]
    public class StringFormattingUsingValuesBenchmarks : StringFormattingBenchmarks
    {
        [Benchmark(Baseline = true)]
        public string ConcatenateValues()
            => "Timeout, ms: " + Timeout
                + ", URL: " + Url
                + ", Request ID: " + RequestId
                + ", Timestamp: " + Timestamp
                + ", Result: " + Result;

        [Benchmark]
        public string ConcatenateValuesInSuboptimalWay()
        {
            var result = "Timeout, ms: " + Timeout;

            result += ", URL: " + Url;
            result += ", Request ID: " + RequestId;
            result += ", Timestamp: " + Timestamp;
            result += ", Result: " + Result;

            return result;
        }

        [Benchmark]
        public string FormatValuesUsingStringFormatMethod()
            //// ReSharper disable once UseStringInterpolation
            => string.Format(
                @"Timeout, ms: {0}, URL: {1}, Request ID: {2}, Timestamp: {3}, Result: {4}",
                Timeout,
                Url,
                RequestId,
                Timestamp,
                Result);

        [Benchmark]
        public string FormatValuesUsingStringInterpolation()
            => $@"Timeout, ms: {Timeout}, URL: {Url}, Request ID: {RequestId}, Timestamp: {Timestamp}, Result: {Result}";

        [Benchmark]
        public string FormatValuesUsingStringBuilderClass()
            => new StringBuilder()
                .Append("Timeout, ms: ").Append(Timeout)
                .Append(", URL: ").Append(Url)
                .Append(", Request ID: ").Append(RequestId)
                .Append(", Timestamp: ").Append(Timestamp)
                .Append(", Result: ").Append(Result)
                .ToString();
    }
}