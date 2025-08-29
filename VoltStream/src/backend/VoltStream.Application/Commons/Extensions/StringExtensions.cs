namespace VoltStream.Application.Commons.Extensions;

public static class StringExtensions
{
    public static bool NormalizedEquals(this string a, string b)
        => a.Equals(b, StringComparison.OrdinalIgnoreCase);

    public static string ToNormalized(this string value) =>
        value.ToUpperInvariant();
}
