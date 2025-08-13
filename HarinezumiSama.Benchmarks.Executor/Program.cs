using System;
using System.IO;
using System.Linq;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;
using HarinezumiSama.Benchmarks.Common;
using HarinezumiSama.Benchmarks.Executor;
using HarinezumiSama.Benchmarks.Omnifactotum.StringExtensions;
using HarinezumiSama.Benchmarks.StringFormatting;

try
{
    var isSpecialCommand = args.Select(static s => s.ToLowerInvariant()).ToArray() is
        ["--help"] or ["--version"] or ["--info"] or ["--list", "flat" or "tree"];

    var resultsDirectoryEnvironmentVariableValue = Environment.GetEnvironmentVariable(InternalHelper.ResultsDirectoryEnvironmentVariableName);

    if (!isSpecialCommand)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"Command line: {Environment.CommandLine}");
        Console.WriteLine($"[ENV] {InternalHelper.ResultsDirectoryEnvironmentVariableName} = '{resultsDirectoryEnvironmentVariableValue}'");
        Console.ResetColor();
        Console.WriteLine();
    }

    var defaultConfig = DefaultConfig.Instance;

    IColumn[] columns =
    [
        // CategoriesColumn.Default,
        // LogicalGroupColumn.Default,
        BaselineColumn.Default,
        StatisticColumn.Mean,
        StatisticColumn.Median,
        StatisticColumn.StdDev
    ];

    // var columnProviders = defaultConfig.GetColumnProviders().ToArray();
    IColumnProvider[] columnProviders =
    [
        //// DefaultColumnProviders.Descriptor,
        CustomDescriptorColumnProvider.Instance,
        DefaultColumnProviders.Job,
        DefaultColumnProviders.Statistics,
        DefaultColumnProviders.Params,
        DefaultColumnProviders.Metrics
    ];

    var analysers = defaultConfig.GetAnalysers().ToArray();

    IExporter[] exporters =
    [
        DefaultExporters.Plain,
        DefaultExporters.Html,
        DefaultExporters.Csv,
        //// DefaultExporters.JsonFull,
        MarkdownExporter.GitHub
        //// DefaultExporters.RPlot
    ];

    var validators = defaultConfig.GetValidators().Append(ReturnValueValidator.FailOnError).Distinct().ToArray();

    var resultsDirectoryPath = string.IsNullOrWhiteSpace(resultsDirectoryEnvironmentVariableValue)
        ? Path.Combine(Path.GetDirectoryName(typeof(Program).Assembly.Location)!, ".benchmarks")
        : resultsDirectoryEnvironmentVariableValue;

    var resolvedResultsDirectoryPath = Path.GetFullPath(resultsDirectoryPath);

    var config = ManualConfig.CreateEmpty()
        .AddLogger(isSpecialCommand ? InternalHelper.DefaultLogger : InternalHelper.Logger)
        .AddColumnProvider(columnProviders)
        .AddColumn(columns)
        .AddExporter(exporters)
        .AddAnalyser(analysers)
        .AddValidator(validators)
        ////.WithOrderer(new DefaultOrderer(SummaryOrderPolicy.FastestToSlowest))
        .WithOrderer(new CustomBenchmarkOrderer())
        .WithCategoryDiscoverer(defaultConfig.CategoryDiscoverer!)
        .WithUnionRule(ConfigUnionRule.Union)
        .WithCultureInfo(defaultConfig.CultureInfo!)
        .WithOptions(defaultConfig.Options | ConfigOptions.DisableParallelBuild | ConfigOptions.JoinSummary)
        .WithSummaryStyle(defaultConfig.SummaryStyle)
        .WithBuildTimeout(defaultConfig.BuildTimeout)
        .WithArtifactsPath(resolvedResultsDirectoryPath);

    if (!isSpecialCommand)
    {
        InternalHelper.Logger.WriteLineInfo($"Benchmark results directory: {resolvedResultsDirectoryPath}");
    }

    var summaries = BenchmarkSwitcher
        .FromTypes([])
        .With(BenchmarkHelper.GetAllBenchmarkTypes<StringFormattingBenchmarks>())
        .With(BenchmarkHelper.GetAllBenchmarkTypes<ToUIStringBenchmarks>())
        .With(BenchmarkHelper.GetAllBenchmarkTypes<ToSecuredUIStringBenchmarks>())
        .With(BenchmarkHelper.GetAllBenchmarkTypes<RegexBenchmarks>())
        .RunAll(config, args)
        .ToArray();

    if (summaries.Length == 0)
    {
        if (isSpecialCommand)
        {
            return 0;
        }

        Console.ResetColor();
        Console.WriteLine();

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("No benchmarks were run.");
        Console.ResetColor();

        Console.WriteLine();

        return 1;
    }

    var failedSummaries = summaries.Where(s => s.ValidationErrors.Any() || s.Reports.Any(r => !r.Success)).ToArray();
    if (failedSummaries.Length != 0)
    {
        var details = string.Join(",\x0020", failedSummaries.Select(s => $"'{s.Title}'"));
        throw new ApplicationException($"The following benchmarks failed: [{details}]");
    }
}
catch (Exception ex)
{
    Console.ResetColor();
    Console.WriteLine();

    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(ex);
    Console.ResetColor();

    Console.WriteLine();
    return sbyte.MaxValue;
}

return 0;