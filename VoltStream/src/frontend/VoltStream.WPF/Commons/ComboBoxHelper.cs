namespace VoltStream.WPF.Commons;

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

public static class ComboBoxHelper
{
    /// <summary>
    /// ComboBox uchun Universal metod ItemsSource ni tekshirish uchun.
    /// </summary>
    /// <param name="comboBox">ComboBox, tekshiriladigon</param>
    /// <param name="e">KeyboardFocusChangedEventArgs</param>
    /// <param name="strInfo">MsgBox ga qo'shish uchun</param>
    /// <param name="allow">ItemsSource da yo'q ma'lumotni qo'shishga ruxsat berish</param>
    public static void BeforeUpdate(object sender, KeyboardFocusChangedEventArgs e, string? strInfo, bool allow = false)
    {
        if (sender is not ComboBox comboBox) return;
        var inputText = comboBox.Text?.Trim();
        if (string.IsNullOrEmpty(inputText)) return;

        var items = comboBox.ItemsSource;
        if (items is null)
        {
            if (allow)
            {
                var result = MessageBox.Show($"{inputText} - {strInfo}: ro'yxatda yo'q. Yangi {strInfo} qo'shilsinmi?",
                    "Tekshiruv", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    return;
                }
                else
                {
                    comboBox.Text = "";
                    comboBox.SelectedItem = null;
                    e.Handled = true;
                    return;
                }
            }
            e.Handled = true;
            MessageBox.Show($"{strInfo} - Ro'yxati bo'sh.", "Tekshiruv");
            return;
        }

        var displayMember = comboBox.DisplayMemberPath;
        bool found = false;

        foreach (var item in items)
        {
            string value = item?.ToString() ?? "";
            if (!string.IsNullOrEmpty(displayMember))
            {
                var prop = item.GetType().GetProperty(displayMember);
                if (prop != null)
                {
                    value = prop.GetValue(item)?.ToString() ?? "";
                }
            }
            if (string.Equals(value, inputText, StringComparison.OrdinalIgnoreCase))
            {
                found = true;
                break;
            }
        }

        if (!found)
        {
            if (allow)
            {
                var result = MessageBox.Show($"{inputText} - {strInfo}: ro'yxatda yo'q. Yangi {strInfo} qo'shilsinmi?",
                    "Tekshiruv", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    return;
                }
                else
                {
                    comboBox.Text = "";
                    comboBox.SelectedItem = null;
                    e.Handled = true;
                    comboBox.IsDropDownOpen = true;
                    return;
                }
            }
            comboBox.Text = "";
            comboBox.SelectedItem = null;
            e.Handled = true;
            MessageBox.Show($"{inputText} - {strInfo}: ro'yxatda yo'q.", "Tekshiruv");
            try
                {
                comboBox.IsDropDownOpen = true;
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
