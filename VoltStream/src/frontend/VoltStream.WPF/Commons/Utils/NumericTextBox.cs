using System;
//using System.Text.RegularExpressions;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Input;
//using System.Globalization;
//using System.Windows.Data;

//namespace VoltStream.WPF.Commons.Utils
//{
//    public static class NumericTextBox
//    {
//        public static bool GetIsNumeric(DependencyObject obj)
//        {
//            return (bool)obj.GetValue(IsNumericProperty);
//        }

//        public static void SetIsNumeric(DependencyObject obj, bool value)
//        {
//            obj.SetValue(IsNumericProperty, value);
//        }

//        public static readonly DependencyProperty IsNumericProperty =
//        DependencyProperty.RegisterAttached(
//            "IsNumeric",
//            typeof(bool),
//            typeof(NumericTextBox),
//            new PropertyMetadata(false, OnIsNumericChanged));

//        private static void OnIsNumericChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
//        {
//            if (d is TextBox textBox)
//            {
//                bool isEnabled = (bool)e.NewValue;

//                if (isEnabled)
//                {
//                    textBox.TextAlignment = TextAlignment.Right; // Выравнивание текста по правому краю
//                    textBox.PreviewTextInput += TextBox_PreviewTextInput;
//                    textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
//                    textBox.GotFocus += TextBox_GotFocus;
//                    textBox.TextChanged += TextBox_TextChanged;
//                    textBox.LostFocus += TextBox_LostFocus;
//                    textBox.SetValue(TextBox.TextProperty, textBox.Text); // Принудительное обновление
//                    textBox.TargetUpdated += TextBox_TargetUpdated!; // Обработка изменений через Binding
//                }
//                else
//                {
//                    textBox.PreviewTextInput -= TextBox_PreviewTextInput;
//                    textBox.PreviewKeyDown -= TextBox_PreviewKeyDown;
//                    textBox.GotFocus -= TextBox_GotFocus;
//                    textBox.TextChanged -= TextBox_TextChanged;
//                    textBox.LostFocus -= TextBox_LostFocus;
//                    textBox.TargetUpdated -= TextBox_TargetUpdated!;
//                }
//            }
//        }

//        private static void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
//        {
//            if (sender is TextBox textBox && !IsValidDecimalInput(textBox.Text, e.Text))
//            {
//                e.Handled = true;
//            }
//        }

//        private static void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
//        {
//            if (e.Key == Key.Space)
//            {
//                e.Handled = true;
//            }
//        }
//        private static void TextBox_GotFocus(object sender, RoutedEventArgs e)
//        {
//            if (sender is TextBox textBox)
//            {
//                // Убираем форматирование, оставляем "сырой" ввод
//                string raw = textBox.Text.Replace(" ", "").Replace(",", "."); // сохраняем как raw
//                if (decimal.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal number))
//                {
//                    textBox.Text = number.ToString(CultureInfo.InvariantCulture); // например: "1234.56"
//                    textBox.CaretIndex = textBox.Text.Length;
//                }
//                textBox.SelectAll();
//            }
//        }
//        private static void TextBox_TargetUpdated(object? sender, DataTransferEventArgs e)
//        {
//            ApplyFormatting(sender as TextBox);
//        }

//        private static void ApplyFormatting(TextBox? textBox)
//        {
//            if (textBox == null) return;

//            string text = textBox.Text.Replace(" ", "").Replace(",", ".");

//            if (decimal.TryParse(text, NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out decimal number))
//            {
//                textBox.Text = FormatNumberWithSpaces(number, 2);
//                textBox.CaretIndex = textBox.Text.Length;
//            }
//            else
//            {
//                textBox.Text = null; // Значение по умолчанию
//            }
//        }

//        private static void TextBox_TextChanged(object sender, TextChangedEventArgs e)
//        {
//            if (sender is TextBox textBox)
//            {
//                string text = textBox.Text;

//                // Удаление ведущих нулей, если их больше одного
//                if (text.StartsWith("00"))
//                {
//                    textBox.Text = text.TrimStart('0');
//                    if (string.IsNullOrEmpty(textBox.Text))
//                    {
//                        textBox.Text = "0";
//                    }
//                    textBox.CaretIndex = textBox.Text.Length;
//                }

//                // Если первый символ - ".", добавляем "0." для корректности
//                if (text.StartsWith(",") || text.StartsWith("."))
//                {
//                    textBox.Text = "0" + text;
//                    textBox.CaretIndex = textBox.Text.Length;
//                }

//                // Замена запятой на точку
//                if (textBox.Text.Contains(","))
//                {
//                    textBox.Text = text.Replace(',', '.');
//                    textBox.CaretIndex = textBox.Text.Length;
//                }
//            }
//        }

//        private static void TextBox_LostFocus(object sender, RoutedEventArgs e)
//        {
//            if (sender is TextBox textBox)
//            {
//                if (decimal.TryParse(textBox.Text.Replace(" ", "").Replace(".", ","),
//                    NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands,
//                    CultureInfo.InvariantCulture, out decimal number))
//                {
//                    textBox.Text = FormatNumberWithSpaces(number, 2);
//                }
//                else
//                {
//                    textBox.Text = null; // "0.00"; // Значение по умолчанию
//                }
//            }
//        }

//        private static bool IsValidDecimalInput(string currentText, string newText)
//        {
//            string fullText = currentText + newText;
//            fullText = fullText.Replace('.', ','); // Замена запятой на точку
//            Regex regex = new Regex(@"^-?\d{0,15}([,]\d{0,2})?$"); // До 15 цифр перед точкой, 2 после
//            return regex.IsMatch(fullText);
//        }

//        private static string FormatNumberWithSpaces(decimal number, int decimalPlaces)
//        {
//            return number.ToString($"N{decimalPlaces}", CultureInfo.InvariantCulture)
//                         .Replace(",", " ") // Разделитель тысяч — пробел
//                         .Replace(".", "."); // Разделитель дробной части — точка
//        }
//    }
//}
using System;
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

        public static bool GetIsNumeric(DependencyObject obj) => (bool)obj.GetValue(IsNumericProperty);
        public static void SetIsNumeric(DependencyObject obj, bool value) => obj.SetValue(IsNumericProperty, value);

        private static void OnIsNumericChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is TextBox textBox)
            {
                if ((bool)e.NewValue)
                {
                    textBox.TextAlignment = TextAlignment.Right;
                    textBox.PreviewTextInput += TextBox_PreviewTextInput;
                    textBox.GotFocus += (s, ev) => textBox.SelectAll();
                    DataObject.AddPastingHandler(textBox, OnPaste);
                }
                else
                {
                    textBox.PreviewTextInput -= TextBox_PreviewTextInput;
                    textBox.GotFocus -= (s, ev) => textBox.SelectAll();
                    DataObject.RemovePastingHandler(textBox, OnPaste);
                }
            }
        }

        private static void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == "-")
            {
                if (sender is TextBox textBox)
                {
                    e.Handled = textBox.CaretIndex != 0 || textBox.Text.Contains("-");
                    return;
                }
            }
            e.Handled = !IsTextNumeric((sender as TextBox)?.Text, e.Text);
        }

        private static void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(DataFormats.Text))
            {
                string text = (string)e.DataObject.GetData(DataFormats.Text);
                if (!IsTextNumeric((sender as TextBox)?.Text, text))
                    e.CancelCommand();
            }
            else
            {
                e.CancelCommand();
            }
        }

        // Проверка с учетом любого разделителя дробной части
        private static bool IsTextNumeric(string? currentText, string newText)
        {
            string decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            MessageBox.Show(decimalSeparator);
            string altSeparator = decimalSeparator == "." ? "," : ".";
            MessageBox.Show(decimalSeparator + " " +altSeparator);
            string fullText = (currentText ?? "") + newText;

            // Заменяем альтернативный разделитель на текущий
            fullText = fullText.Replace(altSeparator, decimalSeparator);

            // Разрешаем только одну запятую/точку
            int sepCount = fullText.Split(decimalSeparator).Length - 1;
            if (sepCount > 1) return false;

            // Проверяем на корректность числа
            string pattern = @"^-?\d*(" + Regex.Escape(decimalSeparator) + @"\d*)?$";
            return Regex.IsMatch(fullText, pattern);
        }
    }
}
