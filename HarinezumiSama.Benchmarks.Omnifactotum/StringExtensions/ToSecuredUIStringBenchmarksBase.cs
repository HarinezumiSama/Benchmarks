using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using HarinezumiSama.Benchmarks.Common;

namespace HarinezumiSama.Benchmarks.Omnifactotum.StringExtensions;

[SimpleJob(RuntimeMoniker.Net48)]
//// [SimpleJob(RuntimeMoniker.NetCoreApp31)]
//// [SimpleJob(RuntimeMoniker.Net50)]
[SimpleJob(RuntimeMoniker.Net60)]
//// [SimpleJob(RuntimeMoniker.Net70)]
[SimpleJob(RuntimeMoniker.Net80)]
//// [SimpleJob(RuntimeMoniker.Net90)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByJob, BenchmarkLogicalGroupRule.ByParams)]
[Orderer(SummaryOrderPolicy.Method, MethodOrderPolicy.Alphabetical)]
[MinIterationTime(120)]
[MemoryDiagnoser]
[SuppressMessage("ReSharper", "ReplaceSliceWithRangeIndexer", Justification = "Multiple target frameworks.")]
public abstract class ToSecuredUIStringBenchmarksBase(int length)
{
    private const string NullValueRepresentation = "null";
    private const char DoubleQuoteChar = '"';

    private static readonly string DoubleQuote = DoubleQuoteChar.ToString();
    private static readonly string DoubleDoubleQuote = DoubleQuote + DoubleQuote;

    private static readonly RepeatableString NoQuotesBaseValue = new("0123456789ABCDEF");
    private static readonly RepeatableString QuotesOnlyBaseValue = new(DoubleQuote);
    private static readonly RepeatableString MixedBaseValue = new("""{ "ClientId": "ce1fc84ca7fc4ba28745768361e8c626", "Name": "'A'-\"b\"::`C`-«d»" }""");

    public IEnumerable<object?> InputValueValues
    {
        get
        {
            if (Length == 0)
            {
                yield return string.Empty;

                yield break;
            }

            var baseTestValues = new[]
            {
                NoQuotesBaseValue,
                QuotesOnlyBaseValue,
                MixedBaseValue
            };

            foreach (var baseTestValue in baseTestValues)
            {
                yield return baseTestValue.GetValue(Length);
            }
        }
    }

    private int Length { get; } = length >= 0 ? length : throw new ArgumentOutOfRangeException(nameof(length), length, "The value cannot be negative.");

    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [ParamsSource(nameof(InputValueValues))]
    public string? InputValue { get; set; }

    [Benchmark(Baseline = true)]
    public string F0_Initial() => Implementation.F0_Initial.ToSecuredUIString(InputValue);

    [Benchmark]
    public string F1_New() => Implementation.F1_New.ToSecuredUIString(InputValue);

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    private static class Implementation
    {
        private const int DefaultMinimumSecuredPartLength = 16;
        private const int DefaultLoggedPartLength = 4;

        internal static class F0_Initial
        {
            public static string ToSecuredUIString(
                string? value,
                int loggedPartLength = DefaultLoggedPartLength,
                int minimumSecuredPartLength = DefaultMinimumSecuredPartLength)
            {
                if (loggedPartLength <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(loggedPartLength), loggedPartLength, "The value must be greater than zero.");
                }

                if (minimumSecuredPartLength <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(minimumSecuredPartLength), minimumSecuredPartLength, "The value must be greater than zero.");
                }

                if (value is null)
                {
                    return NullValueRepresentation;
                }

                var minimumLoggedValueLength = checked(loggedPartLength * 2 + minimumSecuredPartLength);

                var result = value.Length >= minimumLoggedValueLength
                    ? ToUIStringBenchmarksBase.Implementation.F1_UsingStackOrHeapAllocationAndOnePreSearch.ToUIString(
                        $"{value.Substring(0, loggedPartLength)}...{value.Substring(value.Length - loggedPartLength)}")
                    : $"{{ {nameof(value.Length)} = {value.Length} }}";

                return result;
            }
        }

        internal static class F1_New
        {
#if !NETFRAMEWORK
            private const string ShortSecuredUIStringLeftPart = $"{{\x0020{nameof(string.Length)}\x0020=\x0020";
            private const int MaxLengthAsStringLength = 10;
            private const string ShortSecuredUIStringRightPart = "\x0020}";
#endif

            public static string ToSecuredUIString(
                string? value,
                int loggedPartLength = DefaultLoggedPartLength,
                int minimumSecuredPartLength = DefaultMinimumSecuredPartLength)
            {
                if (loggedPartLength <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(loggedPartLength), loggedPartLength, "The value must be greater than zero.");
                }

                if (minimumSecuredPartLength <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(minimumSecuredPartLength), minimumSecuredPartLength, "The value must be greater than zero.");
                }

                if (value is null)
                {
                    return NullValueRepresentation;
                }

                var minimumLoggedValueLength = checked(loggedPartLength * 2 + minimumSecuredPartLength);

                if (value.Length >= minimumLoggedValueLength)
                {
                    return ToUIStringBenchmarksBase.Implementation.F1_UsingStackOrHeapAllocationAndOnePreSearch.ToUIString(
                        $"{value.Substring(0, loggedPartLength)}...{value.Substring(value.Length - loggedPartLength)}");

                    //// throw new NotImplementedException();
                }
                else
                {
#if NETFRAMEWORK
                    return $"{{ {nameof(value.Length)} = {value.Length} }}";
#else
                    var resultBufferSize = ShortSecuredUIStringLeftPart.Length + MaxLengthAsStringLength + ShortSecuredUIStringRightPart.Length;
                    Span<char> resultBuffer = stackalloc char[resultBufferSize];

                    var resultLength = 0;

                    ShortSecuredUIStringLeftPart.AsSpan().CopyTo(resultBuffer.Slice(resultLength));
                    resultLength += ShortSecuredUIStringLeftPart.Length;

                    if (!value.Length.TryFormat(resultBuffer.Slice(resultLength), out var lengthValueCharCount))
                    {
                        throw new InvalidOperationException($"Failed to format the '{nameof(value.Length)}' value.");
                    }

                    resultLength += lengthValueCharCount;

                    ShortSecuredUIStringRightPart.AsSpan().CopyTo(resultBuffer.Slice(resultLength));
                    resultLength += ShortSecuredUIStringRightPart.Length;

                    return new string(resultBuffer.Slice(0, resultLength));
#endif
                }
            }
        }
    }
}