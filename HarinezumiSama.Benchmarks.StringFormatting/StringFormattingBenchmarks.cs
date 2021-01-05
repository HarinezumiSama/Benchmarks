using System;
using System.Globalization;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace HarinezumiSama.Benchmarks.StringFormatting
{
    [SimpleJob(RuntimeMoniker.Net48)]
    [SimpleJob(RuntimeMoniker.NetCoreApp21)]
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [SimpleJob(RuntimeMoniker.NetCoreApp50)]
    [MemoryDiagnoser]
    [ArtifactsPath(@"..\..\..\..\..\benchmarks\" + nameof(StringFormattingBenchmarks))]
    [IterationCount(10)]
    [WarmupCount(3)]
    [ProcessCount(1)]
    public abstract class StringFormattingBenchmarks
    {
        protected static int Timeout { get; } = 42;

        protected static string TimeoutAsString { get; } = Timeout.ToString();

        protected static string Url { get; } = @"https://example.com/";

        protected static string UrlAsString { get; } = Url;

        protected static Guid RequestId { get; } = Guid.Parse("{1314f3cb-af7b-4616-ba2a-8a566becdcea}");

        protected static string RequestIdAsString { get; } = RequestId.ToString();

        protected static DateTimeOffset Timestamp { get; } = new(new DateTime(2021, 1, 5), TimeSpan.FromHours(-5));

        protected static string TimestampAsString { get; } = Timestamp.ToString();

        protected static double Result { get; } = Math.PI;

        protected static string ResultAsString { get; } = Result.ToString(CultureInfo.InvariantCulture);
    }
}