using System;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace HarinezumiSama.Benchmarks.Executor;

internal sealed class CustomTargetMethodColumn : IColumn
{
    private readonly int _commonPrefixLength;

    public CustomTargetMethodColumn(int commonPrefixLength)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(commonPrefixLength);

        _commonPrefixLength = commonPrefixLength;
    }

    public string Id => $"{nameof(CustomTargetMethodColumn)}.{ColumnName}";

    public string ColumnName => Column.Method;

    public bool AlwaysShow => true;

    public ColumnCategory Category => ColumnCategory.Job;

    public int PriorityInCategory => 0;

    public bool IsNumeric => false;

    public UnitType UnitType => UnitType.Dimensionless;

    public string Legend => string.Empty;

    public string GetValue(Summary summary, BenchmarkCase benchmarkCase)
        => string.Concat(
            benchmarkCase.Descriptor.Type.GetFullName().AsSpan(_commonPrefixLength),
            [Type.Delimiter],
            benchmarkCase.Descriptor.WorkloadMethod.Name);

    public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style) => GetValue(summary, benchmarkCase);

    public bool IsDefault(Summary summary, BenchmarkCase benchmarkCase) => false;

    public bool IsAvailable(Summary summary) => true;
}