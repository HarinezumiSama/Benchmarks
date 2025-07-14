using BenchmarkDotNet.Loggers;

namespace HarinezumiSama.Benchmarks.Executor;

internal static class InternalHelper
{
    public const string ResultsDirectoryEnvironmentVariableName = "HARINEZUMISAMA_BENCHMARKS_RESULTS_DIRECTORY";

    public static readonly ILogger DefaultLogger = ConsoleLogger.Default;

    public static readonly ILogger Logger = DynamicallyPrefixedLogger.CreateTimestampPrefixedLogger(DefaultLogger);
}