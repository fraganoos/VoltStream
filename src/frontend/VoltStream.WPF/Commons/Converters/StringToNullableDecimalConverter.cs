namespace VoltStream.WPF.Commons.Converters;

using System.Globalization;
using System.Windows.Data;


public class StringToNullableDecimalConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString()!;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var text = value?.ToString()?.Trim();
        if (string.IsNullOrEmpty(text))
            return null!;

        if (decimal.TryParse(text, NumberStyles.Any, culture, out var result))
            return result;

        return null!;
    }
}
