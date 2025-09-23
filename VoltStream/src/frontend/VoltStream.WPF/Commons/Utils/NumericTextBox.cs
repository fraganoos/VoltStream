using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VoltStream.WPF.Commons.Utils
{
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
                new PropertyMetadata(2)); // По умолчанию 2 знака после запятой

        public static bool GetIsNumeric(DependencyObject obj) => (bool)obj.GetValue(IsNumericProperty);
        public static void SetIsNumeric(DependencyObject obj, bool value) => obj.SetValue(IsNumericProperty, value);

        public static int GetDecimalDigits(DependencyObject obj) => (int)obj.GetValue(DecimalDigitsProperty);
        public static void SetDecimalDigits(DependencyObject obj, int value) => obj.SetValue(DecimalDigitsProperty, value);

        private static void OnIsNumericChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                if ((bool)e.NewValue)
                {
                    textBox.TextAlignment = TextAlignment.Right;
                    textBox.GotFocus += TextBox_GotFocus_SelectAll;
                    textBox.PreviewTextInput += TextBox_PreviewTextInput;
                    textBox.PreviewKeyDown += OnPreviewKeyDown;
                    textBox.TextChanged += TextBox_TextChanged;
                    textBox.LostFocus += TextBox_LostFocus_FormatNumber;
                    DataObject.AddPastingHandler(textBox, OnPaste);
                }
                else
                {
                    textBox.GotFocus -= TextBox_GotFocus_SelectAll;
                    textBox.PreviewTextInput -= TextBox_PreviewTextInput;
                    textBox.PreviewKeyDown -= OnPreviewKeyDown;
                    textBox.TextChanged -= TextBox_TextChanged;
                    textBox.LostFocus -= TextBox_LostFocus_FormatNumber;
                    DataObject.RemovePastingHandler(textBox, OnPaste);
                }
            }
        }
        private static void TextBox_GotFocus_SelectAll(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                // Если текстбокс не в режиме только для чтения и не пустой, выделяем весь текст
                if (!textBox.IsReadOnly && !string.IsNullOrEmpty(textBox.Text))
                {
                    textBox.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        textBox.SelectAll();
                    }), System.Windows.Threading.DispatcherPriority.Input);
                }
            }
        }

        private static void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is not TextBox textBox) return;
            if (textBox.Text.Contains(",") || textBox.Text.Contains("."))
            {
                string decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
                string altSeparator = decimalSeparator == "." ? "," : ".";
                textBox.Text = textBox.Text.Replace(altSeparator, decimalSeparator);
                textBox.CaretIndex = textBox.Text.Length;
            }
        }

        private static void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (sender is not TextBox textBox) return;

            // Разрешить удаление, если выделен весь текст
            if (textBox.SelectionLength == textBox.Text.Length && char.IsDigit(e.Text, 0))
            {
                e.Handled = false;
                return;
            }

            if (e.Text == "-")
            {
                e.Handled = textBox.CaretIndex != 0 || textBox.Text.Contains("-");
                return;
            }
            int decimalDigits = GetDecimalDigits(textBox);
            e.Handled = !IsTextNumeric(textBox.Text, e.Text, decimalDigits);
        }

        // Разрешить удаление при нажатии Delete или Backspace
        private static void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is not TextBox textBox) return;

            if (e.Key == Key.Delete || e.Key == Key.Back)
            {
                // Если выделен весь текст, разрешить удаление
                if (textBox.SelectionLength == textBox.Text.Length)
                {
                    e.Handled = false;
                    return;
                }
                // Если ничего не выделено, разрешить удаление
                if (textBox.SelectionLength == 0)
                {
                    e.Handled = false;
                    return;
                }
            }
        }
        private static void TextBox_LostFocus_FormatNumber(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (double.TryParse(textBox.Text, NumberStyles.Any, CultureInfo.CurrentCulture, out double value))
                {
                    int decimalDigits = GetDecimalDigits(textBox);
                    string format = "N" + decimalDigits;
                    textBox.Text = value.ToString(format, CultureInfo.CurrentCulture);
                }
            }
        }

        // Обработка вставки из буфера обмена
        private static void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (sender is not TextBox textBox)
            {
                e.CancelCommand();
                return;
            }
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                string text = (string)e.DataObject.GetData(DataFormats.Text);
                int decimalDigits = GetDecimalDigits(textBox);
                if (!IsTextNumeric(textBox.Text, text, decimalDigits))
                    e.CancelCommand();
            }
            else
            {
                e.CancelCommand();
            }
        }

        // Проверка с учетом любого разделителя дробной части и ограничения на количество знаков после разделителя
        private static bool IsTextNumeric(string? currentText, string newText, int decimalDigits)
        {
            string decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            string altSeparator = decimalSeparator == "." ? "," : ".";
            string fullText = (currentText ?? "") + newText;

            // Заменяем альтернативный разделитель на текущий
            fullText = fullText.Replace(altSeparator, decimalSeparator);

            // Разрешаем только одну запятую/точку
            int sepCount = fullText.Split(decimalSeparator).Length - 1;
            if (sepCount > 1) return false;

            // Ограничение на количество знаков после разделителя
            int sepIndex = fullText.IndexOf(decimalSeparator, StringComparison.Ordinal);
            if (sepIndex >= 0)
            {
                int afterSep = fullText.Length - sepIndex - 1;
                if (afterSep > decimalDigits)
                    return false;
            }

            // Проверяем на корректность числа
            string pattern = @"^-?\d*(" + Regex.Escape(decimalSeparator) + @"\d{0," + decimalDigits + @"})?$";
            return Regex.IsMatch(fullText, pattern);
        }
    }
}