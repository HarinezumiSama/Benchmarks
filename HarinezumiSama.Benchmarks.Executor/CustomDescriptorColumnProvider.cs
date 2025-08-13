using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Reports;

namespace HarinezumiSama.Benchmarks.Executor;

internal sealed class CustomDescriptorColumnProvider : IColumnProvider
{
    public static readonly IColumnProvider Instance = new CustomDescriptorColumnProvider();

    private CustomDescriptorColumnProvider()
    {
    }

    public IEnumerable<IColumn> GetColumns(Summary summary)
    {
        if (summary.BenchmarksCases.IsDefaultOrEmpty)
        {
            return [];
        }

        var fullNames = summary.BenchmarksCases.Select(static @case => @case.Descriptor.Type.GetFullName()).ToArray();

        var commonPrefixLength = 0;
        while (true)
        {
            var previousPrefixLength = commonPrefixLength;

            var groupings = fullNames
                .Select(
                    s =>
                    {
                        var index = s.IndexOf(Type.Delimiter, commonPrefixLength);
                        var length = index < 0 ? commonPrefixLength : index + 1;

                        return new
                        {
                            Length = length,
                            Prefix = s.Substring(0, length),
                            Name = s.Substring(length)
                        };
                    })
                .GroupBy(static item => item.Prefix, StringComparer.Ordinal)
                .ToArray();

            if (groupings.Length > 1)
            {
                break;
            }

            commonPrefixLength = groupings.Single().First().Length;
            if (commonPrefixLength == previousPrefixLength)
            {
                break;
            }
        }

        return [new CustomTargetMethodColumn(commonPrefixLength)];
    }
}