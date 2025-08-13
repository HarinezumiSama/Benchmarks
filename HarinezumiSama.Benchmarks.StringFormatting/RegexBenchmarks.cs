#if NET7_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;

namespace HarinezumiSama.Benchmarks.StringFormatting;

[SimpleJob(RuntimeMoniker.Net80)]
[BenchmarkCategory(nameof(RegexBenchmarks))]
[MemoryDiagnoser]
[MinIterationTime(120)]
public partial class RegexBenchmarks
{
    private const int RegexTimeoutMilliseconds = 100;

    private const string DefaultWordSplitRegexPattern = """\W+|((?<=\p{Ll})(?=\p{Lu}))""";
    private const RegexOptions DefaultWordSplitRegexOptions = RegexOptions.Singleline;

    private const string NewWordSplitRegexPattern = """(?:\W+|(?<=\p{Ll})(?=\p{Lu}))""";
    private const RegexOptions NewWordSplitRegexOptions = RegexOptions.Singleline | RegexOptions.CultureInvariant;

    public IEnumerable<object> InputValueValues => GetInputValues();

    [ParamsSource(nameof(InputValueValues))]
    [SuppressMessage("ReSharper", "PropertyCanBeMadeInitOnly.Global")]
    public string InputValue { get; set; } = string.Empty;

    public static string[] GetInputValues() => RawInputValues.Append(string.Join('/', RawInputValues)).ToArray();

    //// Benchmarks

    [Benchmark(Baseline = true)]
    public string[] SplitUsingDefaultGeneratedRegex() => GetDefaultWordSplitRegex().Split(InputValue).WhereNotBlank().ToArray();

    [Benchmark]
    public string[] SplitUsingNewGeneratedRegex() => GetNewWordSplitRegex().Split(InputValue).WhereNotBlank().ToArray();

    //// Private methods

    [GeneratedRegex(DefaultWordSplitRegexPattern, DefaultWordSplitRegexOptions, RegexTimeoutMilliseconds)]
    private static partial Regex GetDefaultWordSplitRegex();

    [GeneratedRegex(NewWordSplitRegexPattern, NewWordSplitRegexOptions, RegexTimeoutMilliseconds)]
    private static partial Regex GetNewWordSplitRegex();
}
#endif