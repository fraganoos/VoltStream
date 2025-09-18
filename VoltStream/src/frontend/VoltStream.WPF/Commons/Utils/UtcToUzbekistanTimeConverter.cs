namespace VoltStream.WPF.Commons.Utils;

using System;
using System.Globalization;
using System.Windows.Data;

public class UtcToUzbekistanTimeConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is DateTimeOffset dateTimeOffset)
        {
            // O‘zbekiston vaqti UTC+05:00
            TimeSpan uzbekistanOffset = TimeSpan.FromHours(5);
            DateTimeOffset uzbekistanTime = dateTimeOffset.ToOffset(uzbekistanOffset);
            return uzbekistanTime;
        }
        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // ConvertBack kerak bo‘lmasa, faqat readonly uchun ishlatiladi
        throw new NotImplementedException();
    }
}