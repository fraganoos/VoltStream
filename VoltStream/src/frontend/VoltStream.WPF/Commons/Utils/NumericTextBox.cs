using System.ComponentModel;
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
                new PropertyMetadata(2));

        // Вспомогательное DP для отметки, что мы подписались (чтобы корректно отписывать)
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
            if (sender is not TextBox textBox) return;

            // Нормализуем альтернативный десятичный разделитель
            string decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            string altSeparator = decimalSeparator == "." ? "," : ".";
            if (textBox.Text.Contains(altSeparator))
            {
                var newText = textBox.Text.Replace(altSeparator, decimalSeparator);
                if (newText != textBox.Text)
                {
                    var caret = textBox.CaretIndex;
                    textBox.Text = newText;
                    textBox.CaretIndex = Math.Min(caret, textBox.Text.Length);
                }
            }
        }

        private static void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (sender is not TextBox textBox) { e.Handled = true; return; }

            // Если весь текст выделен и пользователь печатает цифру — разрешаем замену
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

        private static void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is not TextBox textBox) return;

            if (e.Key == Key.Delete || e.Key == Key.Back)
            {
                // Разрешаем удаления — ничего дополнительно не делаем
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

        // Отслеживание программных/Binding-изменений Text
        private static void TextBox_TextPropertyChanged(object? sender, EventArgs e)
        {
            if (sender is not TextBox textBox) return;

            // Форматируем только если контрол НЕ в фокусе (чтобы не мешать редактированию пользователем)
            if (!textBox.IsFocused)
            {
                FormatTextBoxText(textBox);
            }
        }

        // Форматирование: безопасно, с учётом группирующих разделителей
        private static void FormatTextBoxText(TextBox textBox)
        {
            var raw = textBox.Text;
            if (string.IsNullOrWhiteSpace(raw)) return;

            // Удалим группирующие сепараторы перед парсингом
            var groupSep = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
            var decSep = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            string toParse = raw.Replace(groupSep, string.Empty).Trim();

            // Если пользователь случайно ввёл альтернативный десятичный разделитель
            string altSep = decSep == "." ? "," : ".";
            if (toParse.Contains(altSep))
                toParse = toParse.Replace(altSep, decSep);

            if (double.TryParse(toParse, NumberStyles.Number | NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out double value))
            {
                int decimalDigits = GetDecimalDigits(textBox);
                string format = "N" + decimalDigits;
                string formatted = value.ToString(format, CultureInfo.CurrentCulture);

                // Если уже отформатировано — не трогаем
                if (textBox.Text != formatted)
                {
                    // Устанавливаем текст асинхронно, чтобы избежать проблем с текущими событиями/Binding
                    textBox.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        // Дополнительная проверка: возможно между вызовом и выполнением текст изменился
                        if (textBox.Text != formatted)
                            textBox.Text = formatted;
                    }), System.Windows.Threading.DispatcherPriority.Normal);
                }
            }
        }

        // Валидация входа: удаляем группирующие символы перед проверкой
        private static bool IsTextNumeric(string? currentText, string newText, int decimalDigits)
        {
            string decSep = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            string groupSep = CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator;
            string altSep = decSep == "." ? "," : ".";

            // Строим потенциальный финальный текст: вставляем newText в currentText на позицию конца (упрощение)
            // Это простая модель; для более точной симуляции вставки нужно учитывать CaretIndex и Selection
            string combined = (currentText ?? "") + newText;

            // Убираем группирующие сепараторы для валидации
            combined = combined.Replace(groupSep, string.Empty);

            // Нормализуем альтернативный десятичный разделитель
            combined = combined.Replace(altSep, decSep);

            // Разрешаем только 1 десятичный разделитель
            int sepCount = combined.Split(new[] { decSep }, StringSplitOptions.None).Length - 1;
            if (sepCount > 1) return false;

            // Ограничение знаков после десятичного разделителя
            int sepIndex = combined.IndexOf(decSep, StringComparison.Ordinal);
            if (sepIndex >= 0)
            {
                int afterSep = combined.Length - sepIndex - 1;
                if (afterSep > decimalDigits) return false;
            }

            // Паттерн: допускаем опциональный минус, цифры и опциональную дробную часть
            string pattern = @"^-?\d*(" + Regex.Escape(decSep) + @"\d{0," + decimalDigits + @"})?$";
            return Regex.IsMatch(combined, pattern);
        }
    }
}
