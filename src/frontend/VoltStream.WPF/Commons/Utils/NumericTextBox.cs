namespace VoltStream.WPF.Commons.Utils;

using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

public static class NumericTextBox
{
    public static readonly DependencyProperty IsNumericProperty =
        DependencyProperty.RegisterAttached(
            "IsNumeric",
            typeof(bool),
            typeof(NumericTextBox),
            new PropertyMetadata(false, OnIsNumericChanged));

    public static readonly DependencyProperty DecimalDigitsProperty =
        DependencyProperty.RegisterAttached(
            "DecimalDigits",
            typeof(int),
            typeof(NumericTextBox),
            new PropertyMetadata(2));

    private static readonly DependencyProperty IsSubscribedProperty =
        DependencyProperty.RegisterAttached("IsSubscribed",
            typeof(bool),
            typeof(NumericTextBox),
            new PropertyMetadata(false));

    public static bool GetIsNumeric(DependencyObject obj) => (bool)obj.GetValue(IsNumericProperty);
    public static void SetIsNumeric(DependencyObject obj, bool value) => obj.SetValue(IsNumericProperty, value);

    public static int GetDecimalDigits(DependencyObject obj) => (int)obj.GetValue(DecimalDigitsProperty);
    public static void SetDecimalDigits(DependencyObject obj, int value) => obj.SetValue(DecimalDigitsProperty, value);

    private static bool GetIsSubscribed(DependencyObject obj) => (bool)obj.GetValue(IsSubscribedProperty);
    private static void SetIsSubscribed(DependencyObject obj, bool value) => obj.SetValue(IsSubscribedProperty, value);

    private static void OnIsNumericChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TextBox textBox) return;

        var descriptor = DependencyPropertyDescriptor.FromProperty(TextBox.TextProperty, typeof(TextBox));

        if ((bool)e.NewValue)
        {
            if (!GetIsSubscribed(textBox))
            {
                textBox.TextAlignment = TextAlignment.Right;
                textBox.GotFocus += TextBox_GotFocus_SelectAll;
                textBox.PreviewTextInput += TextBox_PreviewTextInput;
                textBox.PreviewKeyDown += OnPreviewKeyDown;
                textBox.TextChanged += TextBox_TextChanged;
                textBox.LostFocus += TextBox_LostFocus_FormatNumber;
                DataObject.AddPastingHandler(textBox, OnPaste);

                descriptor?.AddValueChanged(textBox, TextBox_TextPropertyChanged);
                SetIsSubscribed(textBox, true);
            }
        }
        else
        {
            if (GetIsSubscribed(textBox))
            {
                textBox.GotFocus -= TextBox_GotFocus_SelectAll;
                textBox.PreviewTextInput -= TextBox_PreviewTextInput;
                textBox.PreviewKeyDown -= OnPreviewKeyDown;
                textBox.TextChanged -= TextBox_TextChanged;
                textBox.LostFocus -= TextBox_LostFocus_FormatNumber;
                DataObject.RemovePastingHandler(textBox, OnPaste);

                descriptor?.RemoveValueChanged(textBox, TextBox_TextPropertyChanged);
                SetIsSubscribed(textBox, false);
            }
        }
    }

    private static void TextBox_GotFocus_SelectAll(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            if (!textBox.IsReadOnly && !string.IsNullOrEmpty(textBox.Text))
            {
                textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()), System.Windows.Threading.DispatcherPriority.Input);
            }
        }
    }

    private static void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox textBox || !GetIsNumeric(textBox)) return;

        if (!textBox.IsFocused) return;

        int caretIndex = textBox.CaretIndex;
        string originalText = textBox.Text;

        string cleanText = originalText.Replace(" ", "").Replace(",", ".");

        if (string.IsNullOrEmpty(cleanText)) return;

        string[] parts = cleanText.Split('.');
        string integerPart = parts[0];
        string decimalPart = parts.Length > 1 ? parts[1] : null!;

        string formattedInteger = "";
        int count = 0;
        for (int i = integerPart.Length - 1; i >= 0; i--)
        {
            if (count > 0 && count % 3 == 0)
                formattedInteger = " " + formattedInteger;

            formattedInteger = integerPart[i] + formattedInteger;
            count++;
        }

        string newText = formattedInteger;
        if (cleanText.Contains('.'))
        {
            newText += "." + decimalPart;
        }

        if (newText != originalText)
        {
            textBox.Text = newText;

            int spacesBeforeCaret = originalText[..Math.Min(caretIndex, originalText.Length)].Count(c => c == ' ');
            int cleanCharsBeforeCaret = Math.Min(caretIndex, originalText.Length) - spacesBeforeCaret;

            int newCaretIndex = 0;
            int charsCount = 0;
            while (newCaretIndex < newText.Length && charsCount < cleanCharsBeforeCaret)
            {
                if (newText[newCaretIndex] != ' ')
                    charsCount++;
                newCaretIndex++;
            }

            textBox.CaretIndex = newCaretIndex;
        }
    }

    private static void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        if (sender is not TextBox textBox) return;

        bool isDigit = char.IsDigit(e.Text, 0);
        bool isSeparator = e.Text == "." || e.Text == ",";

        if (!isDigit && !isSeparator)
        {
            e.Handled = true;
            return;
        }

        if (isSeparator && textBox.Text.Contains('.'))
        {
            e.Handled = true;
        }
    }

    private static void OnPreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox _) return;

        if (e.Key == Key.Delete || e.Key == Key.Back)
        {
            e.Handled = false;
        }
    }

    private static void TextBox_LostFocus_FormatNumber(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            FormatTextBoxText(textBox);
        }
    }

    private static void OnPaste(object sender, DataObjectPastingEventArgs e)
    {
        if (sender is not TextBox textBox)
        {
            e.CancelCommand();
            return;
        }

        if (e.DataObject.GetDataPresent(DataFormats.Text))
        {
            string paste = (string)e.DataObject.GetData(DataFormats.Text);
            int decimalDigits = GetDecimalDigits(textBox);
            if (!IsTextNumeric(textBox.Text, paste, decimalDigits))
                e.CancelCommand();
        }
        else
        {
            e.CancelCommand();
        }
    }

    private static void TextBox_TextPropertyChanged(object? sender, EventArgs e)
    {
        if (sender is not TextBox textBox) return;

        if (!textBox.IsFocused)
        {
            FormatTextBoxText(textBox);
        }
    }

    private static void FormatTextBoxText(TextBox textBox)
    {
        string raw = textBox.Text.Replace(" ", "").Replace(",", ".");
        if (double.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
        {
            int digits = GetDecimalDigits(textBox);

            var nfi = new NumberFormatInfo
            {
                NumberGroupSeparator = " ",
                NumberDecimalSeparator = ".",
                NumberGroupSizes = [3]
            };

            textBox.Text = value.ToString("N" + digits, nfi);
        }
    }

    private static bool IsTextNumeric(string? currentText, string newText, int decimalDigits)
    {
        string combined = (currentText ?? "").Replace(" ", "").Replace(",", ".") + newText.Replace(",", ".");

        int sepCount = combined.Split('.').Length - 1;
        if (sepCount > 1) return false;

        int sepIndex = combined.IndexOf('.');
        if (sepIndex >= 0)
        {
            int afterSep = combined.Length - sepIndex - 1;
            if (afterSep > decimalDigits) return false;
        }

        string pattern = @"^-?\d*\.?\d{0," + decimalDigits + @"}$";
        return Regex.IsMatch(combined.Replace(" ", ""), pattern);
    }
}
