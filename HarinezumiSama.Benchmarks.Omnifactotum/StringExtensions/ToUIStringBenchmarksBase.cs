using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using HarinezumiSama.Benchmarks.Common;

namespace HarinezumiSama.Benchmarks.Omnifactotum.StringExtensions;

//// [SimpleJob(RuntimeMoniker.Net48)]
//// [SimpleJob(RuntimeMoniker.NetCoreApp31)]
//// [SimpleJob(RuntimeMoniker.Net50)]
//// [SimpleJob(RuntimeMoniker.Net60)]
//// [SimpleJob(RuntimeMoniker.Net70)]
[SimpleJob(RuntimeMoniker.Net80)]
//// [SimpleJob(RuntimeMoniker.Net90)]
[GroupBenchmarksBy(BenchmarkLogicalGroupRule.ByJob, BenchmarkLogicalGroupRule.ByParams)]
[Orderer(SummaryOrderPolicy.Method, MethodOrderPolicy.Alphabetical)]
[MinIterationTime(120)]
[MemoryDiagnoser]
[SuppressMessage("ReSharper", "ReplaceSliceWithRangeIndexer", Justification = "Multiple target frameworks.")]
public abstract class ToUIStringBenchmarksBase(int length)
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
    public string F0_StringReplaceAndConcat() => Implementation.F0_StringReplaceAndConcat.ToUIString(InputValue);

    [Benchmark]
    public string F1_UsingStackOrHeapAllocationAndOnePreSearch()
        => Implementation.F1_UsingStackOrHeapAllocationAndOnePreSearch.ToUIString(InputValue);

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal static class Implementation
    {
        internal static class F0_StringReplaceAndConcat
        {
            public static string ToUIString(string? value)
                => value is null
                    ? NullValueRepresentation
                    : string.Concat(
                        DoubleQuote,
                        value.Replace(DoubleQuote, DoubleDoubleQuote),
                        DoubleQuote);
        }

        internal static class F1_UsingStackOrHeapAllocationAndOnePreSearch
        {
            private static readonly string SingleDoubleQuoteResult = new(DoubleQuoteChar, 4);

            public static string ToUIString(string? value)
            {
                switch (value)
                {
                    case null:
                        return NullValueRepresentation;

                    case { Length: 0 }:
                        return DoubleDoubleQuote;

#if NETFRAMEWORK
                    //// ReSharper disable once MergeIntoPattern
                    case { Length: 1 } when value[0] == DoubleQuoteChar:
                        return SingleDoubleQuoteResult;

                    case { Length: 1 }:
                        {
                            var ch = value[0];
                            ReadOnlySpan<char> span = stackalloc char[] { DoubleQuoteChar, ch, DoubleQuoteChar };
                            unsafe
                            {
                                fixed (char* spanPointer = span)
                                {
                                    return new string(spanPointer, 0, span.Length);
                                }
                            }
                        }
#else
                    case [DoubleQuoteChar]:
                        return SingleDoubleQuoteResult;

                    case [var ch]:
                        return string.Create(
                            3,
                            ch,
                            static (span, ch) =>
                            {
                                span[0] = DoubleQuoteChar;
                                span[1] = ch;
                                span[2] = DoubleQuoteChar;
                            });
#endif
                }

                const int MaxStackBufferLength = 1024 / sizeof(char);

                var valueSpan = value.AsSpan();

                var firstDoubleQuoteCharIndex = valueSpan.IndexOf(DoubleQuoteChar);
                if (firstDoubleQuoteCharIndex < 0)
                {
                    var resultValueLength = value.Length + 2;
#if NETFRAMEWORK
                    var resultBuffer = resultValueLength > MaxStackBufferLength ? new char[resultValueLength] : stackalloc char[resultValueLength];

                    var resultLength = 0;
                    resultBuffer[resultLength++] = DoubleQuoteChar;

                    valueSpan.CopyTo(resultBuffer.Slice(resultLength));
                    resultLength += valueSpan.Length;

                    resultBuffer[resultLength++] = DoubleQuoteChar;

                    unsafe
                    {
                        fixed (char* resultBufferPointer = resultBuffer)
                        {
                            return new string(resultBufferPointer, 0, resultLength);
                        }
                    }
#else
                    return string.Create(
                        resultValueLength,
#if NET9_0_OR_GREATER
                        value.AsSpan(),
#else
                        value,
#endif
                        static (span, state) =>
                        {
                            span[0] = DoubleQuoteChar;
#if NET6_0_OR_GREATER
                            state.CopyTo(span.Slice(1));
#else
                            state.AsSpan().CopyTo(span.Slice(1));
#endif
                            span[^1] = DoubleQuoteChar;
                        });
#endif
                }
                //// ReSharper disable once RedundantIfElseBlock :: False detection: `resultBuffer` is declared in each scope
                else
                {
                    var requiredBufferLength = value.Length * 2 + 2;
                    var resultBuffer = requiredBufferLength > MaxStackBufferLength ? new char[requiredBufferLength] : stackalloc char[requiredBufferLength];

                    var length = 0;
                    resultBuffer[length++] = DoubleQuoteChar;

                    var copiedSpan = valueSpan.Slice(0, firstDoubleQuoteCharIndex + 1);
                    copiedSpan.CopyTo(resultBuffer.Slice(length));
                    length += copiedSpan.Length;

                    resultBuffer[length++] = DoubleQuoteChar;

                    for (var index = firstDoubleQuoteCharIndex + 1; index < valueSpan.Length; index++)
                    {
                        var ch = valueSpan[index];

                        resultBuffer[length++] = ch;
                        if (ch == DoubleQuoteChar)
                        {
                            resultBuffer[length++] = DoubleQuoteChar;
                        }
                    }

                    resultBuffer[length++] = DoubleQuoteChar;

#if NETFRAMEWORK
                    unsafe
                    {
                        fixed (char* resultBufferPointer = resultBuffer)
                        {
                            return new string(resultBufferPointer, 0, length);
                        }
                    }
#else
                    return new string(resultBuffer.Slice(0, length));
#endif
                }
            }
        }
    }
}