namespace VoltStream.WPF.Commons.UserControls;

using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

public partial class UserCalendar : UserControl
{
    public static readonly DependencyProperty SelectedDateProperty =
        DependencyProperty.Register(
            nameof(SelectedDate),
            typeof(DateTime?),
            typeof(UserCalendar),
            new PropertyMetadata(null, OnSelectedDateChanged));

    public UserCalendar()
    {
        InitializeComponent();
        TextBox.PreviewTextInput += DateTextBox_PreviewTextInput;
        TextBox.TextChanged += DateTextBox_TextChanged;
        TextBox.LostFocus += DateTextBox_LostFocus;
    }

    private void DateTextBox_LostFocus(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(TextBox.Text))
        {
            SelectedDate = null;
            return;
        }

        if (TextBox.Text.Length == 10)
        {
            string[] parts = TextBox.Text.Split('.');
            if (parts.Length == 3)
            {
                _ = int.TryParse(parts[0], out int day);
                _ = int.TryParse(parts[1], out int month);
                _ = int.TryParse(parts[2], out int year);

                year = year < 1 ? DateTime.Now.Year : year;

                month = Math.Clamp(month, 1, 12);

                int daysInMonth = DateTime.DaysInMonth(year, month);
                day = Math.Clamp(day, 1, daysInMonth);

                DateTime correctedDate = new(year, month, day);

                SelectedDate = correctedDate;
            }
        }
        else
        {
            TextBox.Text = SelectedDate?.ToString("dd.MM.yyyy") ?? string.Empty;
        }
    }

    public DateTime? SelectedDate
    {
        get => (DateTime?)GetValue(SelectedDateProperty);
        set => SetValue(SelectedDateProperty, value);
    }

    private static void OnSelectedDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is UserCalendar userCalendar)
        {
            var newDate = (DateTime?)e.NewValue;

            userCalendar.calendar.SelectedDate = newDate;

            userCalendar.TextBox.Text = newDate?.ToString("dd.MM.yyyy") ?? string.Empty;
        }
    }

    private void DateTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            if (textBox.Text.Length is 2 or 5 && !textBox.Text.EndsWith("."))
            {
                textBox.Text += ".";
                textBox.CaretIndex = textBox.Text.Length;
            }

            // Sana to'liq bo'lganda
            if (textBox.Text.Length == 10 && DateTime.TryParseExact(
                textBox.Text, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDate))
            {
                if (SelectedDate != parsedDate)
                    SelectedDate = parsedDate;
            }
            // Sana o'chirilganda
            else if (textBox.Text.Length < 10 && SelectedDate != null)
            {
                SelectedDate = null;
            }
        }
    }

    private void OpenCalendar_Click(object sender, RoutedEventArgs e) => Popup.IsOpen = true;

    private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
    {
        if (calendar.SelectedDate.HasValue)
        {
            SelectedDate = calendar.SelectedDate.Value;
            Popup.IsOpen = false;
        }
    }

    private void DateTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        e.Handled = TextBox.Text.Length >= 10 || !DateInputRegex().IsMatch(e.Text);
    }

    [GeneratedRegex("[0-9]")]
    private static partial Regex DateInputRegex();
}