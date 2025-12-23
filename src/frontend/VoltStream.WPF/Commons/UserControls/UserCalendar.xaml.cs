namespace VoltStream.WPF.Commons.UserControls;

using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

/// <summary>
/// Логика взаимодействия для UserCalendar.xaml
/// </summary>
public partial class UserCalendar : UserControl
{
    public static readonly DependencyProperty SelectedDateProperty =
        DependencyProperty.Register("SelectedDate", typeof(DateTime?), typeof(UserCalendar), new PropertyMetadata(DateTime.Now, OnSelectedDateChanged));

    public UserCalendar()
    {
        InitializeComponent();
        TextBox.PreviewTextInput += DateTextBox_PreviewTextInput;
        TextBox.TextChanged += DateTextBox_TextChanged;
        Loaded += UserCalendar_Loaded;
        SetDefaultDate();
    }

    public DateTime? SelectedDate
    {
        get => (DateTime?)GetValue(SelectedDateProperty);
        set => SetValue(SelectedDateProperty, value);
    }

    private void UserCalendar_Loaded(object sender, RoutedEventArgs e)
    {
        if (SelectedDate is DateTime date)
            TextBox.Text = date.ToString("dd.MM.yyyy");
    }

    private static void OnSelectedDateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is UserCalendar userCalendar)
        {
            if (e.NewValue is DateTime newDate)
            {
                userCalendar.TextBox.Text = newDate.ToString("dd.MM.yyyy");
            }
            else
            {
                userCalendar.TextBox.Text = string.Empty;
            }
        }
    }

    private void SetDefaultDate()
    {
        if (SelectedDate == null)
        {
            SelectedDate = DateTime.Now.Date;
        }
    }

    private void DateTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        var textBox = (TextBox)sender;

        if (!string.IsNullOrEmpty(textBox.SelectedText))
        {
            _ = textBox.Text.Remove(textBox.SelectionStart, textBox.SelectionLength)
                            .Insert(textBox.SelectionStart, e.Text);
        }
        else
        {
            _ = textBox.Text.Insert(textBox.CaretIndex, e.Text);
        }

        if (TextBox.Text.Length > 10)
        {
            e.Handled = true;
        }
        else
        {
            e.Handled = !IsValidDateInput(e.Text);
        }
    }

    private void DateTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            if (textBox.Text.Length == 2 || (textBox.Text.Length == 5 && textBox.Text[2] != '.'))
            {
                textBox.Text += ".";
                textBox.CaretIndex = textBox.Text.Length;
            }
            if (textBox.Text.Length == 5 && textBox.Text[2] == '.')
            {
                textBox.Text += ".";
                textBox.CaretIndex = textBox.Text.Length;
            }

            if (textBox.Text.Length == 10 && DateTime.TryParseExact(
                textBox.Text,
                "dd.MM.yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime parsedDate))
            {
                textBox.TextChanged -= DateTextBox_TextChanged;

                SelectedDate = parsedDate;

                textBox.TextChanged += DateTextBox_TextChanged;
            }
            else if (textBox.Text.Length < 10)
            {
                if (SelectedDate.HasValue)
                {
                    textBox.TextChanged -= DateTextBox_TextChanged;

                    SetValue(SelectedDateProperty, null);

                    textBox.TextChanged += DateTextBox_TextChanged;
                }
            }
        }
    }

    private void OpenCalendar_Click(object sender, RoutedEventArgs e)
    {
        Popup.IsOpen = true;
    }

    private void Calendar_SelectedDatesChanged(object sender, SelectionChangedEventArgs e)
    {
        if (calendar.SelectedDate.HasValue)
        {
            SelectedDate = calendar.SelectedDate.Value;
            Popup.IsOpen = false;
        }
    }

    private static bool IsValidDateInput(string input) => DateInputRegex().IsMatch(input);

    private static bool IsValidDateFormat(string input) => DateFormatRegex().IsMatch(input);


    [GeneratedRegex("[0-9]", RegexOptions.Compiled)]
    private static partial Regex DateInputRegex();

    [GeneratedRegex(@"^(?:\d{2}\.\d{2}\.\d{2,4})?$", RegexOptions.Compiled)]
    private static partial Regex DateFormatRegex();
}