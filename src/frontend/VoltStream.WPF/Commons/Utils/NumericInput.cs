namespace VoltStream.WPF.Commons.Utils;

using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

public static class NumericInput
{
    public static readonly DependencyProperty IsNumericProperty =
        DependencyProperty.RegisterAttached(
            "IsNumeric",
            typeof(bool),
            typeof(NumericInput),
            new PropertyMetadata(false, OnIsNumericChanged));

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.RegisterAttached(
            "Value",
            typeof(object),
            typeof(NumericInput),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged));

    public static readonly DependencyProperty PrecisionProperty =
        DependencyProperty.RegisterAttached(
            "Precision",
            typeof(int?),
            typeof(NumericInput),
            new PropertyMetadata(null));

    public static bool GetIsNumeric(DependencyObject obj) => (bool)obj.GetValue(IsNumericProperty);
    public static void SetIsNumeric(DependencyObject obj, bool value) => obj.SetValue(IsNumericProperty, value);

    public static object GetValue(DependencyObject obj) => obj.GetValue(ValueProperty);
    public static void SetValue(DependencyObject obj, object value) => obj.SetValue(ValueProperty, value);

    public static int? GetPrecision(DependencyObject obj) => (int?)obj.GetValue(PrecisionProperty);
    public static void SetPrecision(DependencyObject obj, int? value) => obj.SetValue(PrecisionProperty, value);

    private static readonly DependencyProperty IsSubscribedProperty =
        DependencyProperty.RegisterAttached("IsSubscribed", typeof(bool), typeof(NumericInput), new PropertyMetadata(false));

    private static bool GetIsSubscribed(DependencyObject obj) => (bool)obj.GetValue(IsSubscribedProperty);
    private static void SetIsSubscribed(DependencyObject obj, bool value) => obj.SetValue(IsSubscribedProperty, value);

    private static readonly DependencyProperty IsInternalChangeProperty =
        DependencyProperty.RegisterAttached("IsInternalChange", typeof(bool), typeof(NumericInput), new PropertyMetadata(false));

    private static bool GetIsInternalChange(DependencyObject obj) => (bool)obj.GetValue(IsInternalChangeProperty);
    private static void SetIsInternalChange(DependencyObject obj, bool value) => obj.SetValue(IsInternalChangeProperty, value);

    private static void OnIsNumericChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TextBox textBox) return;

        if ((bool)e.NewValue)
        {
            if (!GetIsSubscribed(textBox))
            {
                textBox.TextAlignment = TextAlignment.Right;
                textBox.GotFocus += OnGotFocus;
                textBox.PreviewTextInput += OnPreviewTextInput;
                textBox.TextChanged += OnTextChanged;
                textBox.LostFocus += OnLostFocus;
                DataObject.AddPastingHandler(textBox, OnPaste);

                SetIsSubscribed(textBox, true);

                FormatValue(textBox);
            }
        }
        else
        {
            if (GetIsSubscribed(textBox))
            {
                textBox.GotFocus -= OnGotFocus;
                textBox.PreviewTextInput -= OnPreviewTextInput;
                textBox.TextChanged -= OnTextChanged;
                textBox.LostFocus -= OnLostFocus;
                DataObject.RemovePastingHandler(textBox, OnPaste);

                SetIsSubscribed(textBox, false);
            }
        }
    }

    private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TextBox textBox || !GetIsNumeric(textBox)) return;

        if (GetIsInternalChange(textBox)) return;

        if (!textBox.IsFocused)
        {
            FormatValue(textBox);
        }
    }

    private static void FormatValue(TextBox textBox)
    {
        SetIsInternalChange(textBox, true);

        var value = GetValue(textBox);
        var precision = GetPrecision(textBox);

        if (value == null)
        {
            textBox.Text = string.Empty;
        }
        else
        {
            decimal decimalValue = ConvertToDecimal(value);
            string format = precision.HasValue ? $"N{precision.Value}" : "N" + GetOptimalDecimalPlaces(decimalValue);

            var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
            culture.NumberFormat.NumberGroupSeparator = " ";
            culture.NumberFormat.NumberDecimalSeparator = ".";

            textBox.Text = decimalValue.ToString(format, culture);
        }

        SetIsInternalChange(textBox, false);
    }

    private static int GetOptimalDecimalPlaces(decimal value)
    {
        string text = value.ToString(CultureInfo.InvariantCulture);
        int index = text.IndexOf('.');
        return index == -1 ? 0 : text.Length - index - 1;
    }

    private static void OnGotFocus(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox textBox && !textBox.IsReadOnly && !string.IsNullOrEmpty(textBox.Text))
        {
            textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()), System.Windows.Threading.DispatcherPriority.Input);
        }
    }

    private static void OnPreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        if (sender is not TextBox textBox) return;

        if (!IsDigitOrSeparator(e.Text))
        {
            e.Handled = true;
            return;
        }

        string fullText = GetFullText(textBox, e.Text);

        if (fullText.Count(c => c == '.') > 1)
        {
            e.Handled = true;
            return;
        }

        var precision = GetPrecision(textBox);
        if (precision.HasValue)
        {
            int separatorIndex = fullText.IndexOf('.');
            if (separatorIndex >= 0 && fullText.Length - separatorIndex - 1 > precision.Value)
            {
                e.Handled = true;
            }
        }
    }

    private static void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is not TextBox textBox || !GetIsNumeric(textBox)) return;
        if (GetIsInternalChange(textBox)) return;

        SetIsInternalChange(textBox, true);

        int caretIndex = textBox.CaretIndex;
        string originalText = textBox.Text;
        string cleanValues = originalText.Replace(" ", "");

        SetIsInternalChange(textBox, false);

        if (string.IsNullOrEmpty(cleanValues))
        {
            SetValue(textBox, null!);
        }
        else if (cleanValues == ".")
        {
        }
        else if (decimal.TryParse(cleanValues, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal val))
        {
            var bindingExpression = textBox.GetBindingExpression(ValueProperty);
            if (bindingExpression?.ResolvedSourcePropertyName != null)
            {
                var source = bindingExpression.ResolvedSource;
                var propertyInfo = source?.GetType().GetProperty(bindingExpression.ResolvedSourcePropertyName);
                var targetType = propertyInfo?.PropertyType;

                var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

                if (underlyingType == typeof(int))
                {
                    SetValue(textBox, (int)Math.Round(val));
                }
                else if (underlyingType == typeof(long))
                {
                    SetValue(textBox, (long)Math.Round(val));
                }
                else if (underlyingType == typeof(double))
                {
                    SetValue(textBox, (double)val);
                }
                else if (underlyingType == typeof(float))
                {
                    SetValue(textBox, (float)val);
                }
                else
                {
                    SetValue(textBox, val);
                }
            }
            else
            {
                SetValue(textBox, val);
            }
        }

        SetIsInternalChange(textBox, true);

        string formattedText = ApplyFormatting(cleanValues);

        if (formattedText != originalText)
        {
            textBox.Text = formattedText;
            RestoreCaretPosition(textBox, originalText, formattedText, caretIndex);
        }

        SetIsInternalChange(textBox, false);
    }

    private static string ApplyFormatting(string cleanText)
    {
        if (string.IsNullOrEmpty(cleanText)) return string.Empty;
        if (cleanText == ".") return ".";

        string[] parts = cleanText.Split('.');
        string integerPart = parts[0];
        string decimalPart = parts.Length > 1 ? parts[1] : null!;

        string formattedInteger = "";
        if (!string.IsNullOrEmpty(integerPart))
        {
            if (decimal.TryParse(integerPart, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal val))
            {
                var culture = (CultureInfo)CultureInfo.InvariantCulture.Clone();
                culture.NumberFormat.NumberGroupSeparator = " ";
                formattedInteger = val.ToString("N0", culture);
            }
            else
            {
                formattedInteger = integerPart;
            }
        }

        if (cleanText.EndsWith("."))
        {
            return formattedInteger + ".";
        }

        if (decimalPart != null)
        {
            return formattedInteger + "." + decimalPart;
        }

        return formattedInteger;
    }

    private static void RestoreCaretPosition(TextBox textBox, string original, string formatted, int originalCaret)
    {
        int digitsBefore = 0;
        for (int i = 0; i < originalCaret && i < original.Length; i++)
        {
            if (original[i] != ' ') digitsBefore++;
        }

        int newCaret = 0;
        int digitsSeen = 0;

        while (newCaret < formatted.Length && digitsSeen < digitsBefore)
        {
            if (formatted[newCaret] != ' ') digitsSeen++;
            newCaret++;
        }

        while (newCaret < formatted.Length && formatted[newCaret] != ' ' && !char.IsDigit(formatted[newCaret]) && formatted[newCaret] != '.')
        {
            newCaret++;
        }

        textBox.CaretIndex = newCaret;
    }

    private static void OnLostFocus(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            FormatValue(textBox);
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
            if (!IsTextValid(textBox, paste))
            {
                e.CancelCommand();
            }
        }
        else
        {
            e.CancelCommand();
        }
    }

    private static bool IsTextValid(TextBox textBox, string text)
    {
        return Regex.IsMatch(text, @"^[\d\s\.]+$");
    }

    private static bool IsDigitOrSeparator(string text)
    {
        return Regex.IsMatch(text, @"^[\d\.,]+$");
    }

    private static string GetFullText(TextBox textBox, string input)
    {
        string text = textBox.Text;
        int index = textBox.CaretIndex;
        if (index < 0) index = 0;
        if (index > text.Length) index = text.Length;
        return text.Insert(index, input).Replace(" ", "");
    }

    private static decimal ConvertToDecimal(object value)
    {
        if (value == null) return 0;
        try
        {
            return Convert.ToDecimal(value, CultureInfo.InvariantCulture);
        }
        catch
        {
            return 0;
        }
    }
}

