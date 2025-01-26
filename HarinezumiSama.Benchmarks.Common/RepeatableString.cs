using System;
using System.Text;

namespace HarinezumiSama.Benchmarks.Common;

public sealed record RepeatableString
{
    public RepeatableString(string baseValue)
    {
        if (string.IsNullOrEmpty(baseValue))
        {
            throw new ArgumentException("The value can be neither empty string nor null.", nameof(baseValue));
        }

        BaseValue = baseValue;
    }

    private string BaseValue { get; }

    public string GetValue(int length)
    {
        switch (length)
        {
            case < 0:
                throw new ArgumentOutOfRangeException(nameof(length), length, "The value cannot be negative.");

            case 0:
                return string.Empty;
        }

        if (length == BaseValue.Length)
        {
            return BaseValue;
        }

        var repeatCount = Math.DivRem(length, BaseValue.Length, out var remainder);

        return repeatCount == 0
            ? BaseValue.Substring(0, remainder)
            : new StringBuilder(length).Insert(0, BaseValue, repeatCount).Append(BaseValue, 0, remainder).ToString();
    }
}