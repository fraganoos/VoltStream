namespace VoltStream.Application.Commons.Extensions;

using System.Text.Json;

public static class ConversionHelper
{
    public static object? TryConvert(object value, Type targetType)
    {
        targetType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (value is JsonElement json)
            value = json.ExtractJsonValue()
                ?? throw new InvalidCastException($"'{targetType.Name}' turiga null qiymatni o‘tkazib bo‘lmaydi.");

        if (targetType == typeof(Guid))
            return Guid.Parse(value?.ToString()!);

        if (targetType == typeof(DateTime))
            return DateTime.Parse(value?.ToString()!);

        if (targetType.IsEnum)
            return Enum.Parse(targetType, value?.ToString()!, ignoreCase: true);

        if (value is IConvertible)
            return Convert.ChangeType(value, targetType);

        throw new InvalidCastException($"Qiymat '{value}' turga o‘zgartirib bo‘lmaydi: {targetType.Name}");
    }

    public static object? ExtractJsonValue(this JsonElement json) => json.ValueKind switch
    {
        JsonValueKind.String => json.GetString(),
        JsonValueKind.Number => json.TryGetInt64(out var l) ? l : json.GetDouble(),
        JsonValueKind.True => true,
        JsonValueKind.False => false,
        JsonValueKind.Null => null,
        _ => json.ToString()
    };
}
