namespace VoltStream.Application.Commons.Extensions;

public static class StringExtensions
{
    public static string ToNormalized(this string? value) =>
        value?.Trim()?.ToUpperInvariant()!;

    public static string Trimmer(this string value, int length)
    {
        if (string.IsNullOrEmpty(value) || length <= 3)
            return value;

        if (value.Length <= length)
            return value;

        return string.Concat(value.AsSpan(0, length - 3), "...");
    }
}
