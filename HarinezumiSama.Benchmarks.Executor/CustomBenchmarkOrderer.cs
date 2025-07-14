using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace HarinezumiSama.Benchmarks.Executor;

internal sealed class CustomBenchmarkOrderer : IOrderer
{
    private static readonly IOrderer Default = DefaultOrderer.Instance;

    public IEnumerable<BenchmarkCase> GetExecutionOrder(ImmutableArray<BenchmarkCase> benchmarksCase, IEnumerable<BenchmarkLogicalGroupRule>? order = null)
        => Default.GetExecutionOrder(benchmarksCase, order);

    public IEnumerable<BenchmarkCase> GetSummaryOrder(ImmutableArray<BenchmarkCase> benchmarksCases, Summary summary)
        => Default.GetSummaryOrder(benchmarksCases, summary);

    public string? GetHighlightGroupKey(BenchmarkCase benchmarkCase) => Default.GetHighlightGroupKey(benchmarkCase);

    public string? GetLogicalGroupKey(ImmutableArray<BenchmarkCase> allBenchmarksCases, BenchmarkCase benchmarkCase)
        => Default.GetLogicalGroupKey(allBenchmarksCases, benchmarkCase);

    public IEnumerable<IGrouping<string, BenchmarkCase>> GetLogicalGroupOrder(
        IEnumerable<IGrouping<string, BenchmarkCase>> logicalGroups,
        IEnumerable<BenchmarkLogicalGroupRule>? order = null)
        => Default.GetLogicalGroupOrder(logicalGroups, order);

    public bool SeparateLogicalGroups => Default.SeparateLogicalGroups;
}