namespace VoltStream.WPF.Commons;

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

public static class ComboBoxHelper
{
    /// <summary>
    /// Проверка введённого текста в ComboBox.
    /// </summary>
    /// <returns>
    /// true  – если введённый текст принят (есть в списке или разрешено добавить),  
    /// false – если введённый текст отвергнут.
    /// </returns>
    public static bool BeforeUpdate(
        object sender,
        KeyboardFocusChangedEventArgs e,
        string? strInfo,
        bool allow = false)
    {
        if (sender is not ComboBox comboBox) return false;
        var inputText = comboBox.Text?.Trim();
        if (string.IsNullOrEmpty(inputText)) return false;

        var items = comboBox.ItemsSource;
        if (items is null)
        {
            if (allow)
            {
                var result = MessageBox.Show($"{inputText} - {strInfo}: ro'yxatda yo'q. Yangi {strInfo} qo'shilsinmi?",
                    "Tekshiruv", MessageBoxButton.YesNo);
                if (result == MessageBoxResult.Yes)
                    return true;

                comboBox.Text = "";
                comboBox.SelectedItem = null;
                e.Handled = true;
                return false;
            }
            e.Handled = true;
            MessageBox.Show($"{strInfo} - Ro'yxati bo'sh.", "Tekshiruv");
            return false;
        }

        var displayMember = comboBox.DisplayMemberPath;
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
                return false; // найден в списке
            }
        }

        // не найден
        if (allow)
        {
            var result = MessageBox.Show($"{inputText} - {strInfo}: ro'yxatda yo'q. Yangi {strInfo} qo'shilsinmi?",
                "Tekshiruv", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
                return true;

            comboBox.Text = "";
            comboBox.SelectedItem = null;
            comboBox.SelectedValue = null;
            e.Handled = true;
            try { comboBox.IsDropDownOpen = true; } catch { }
            return false;
        }

        comboBox.Text = "";
        comboBox.SelectedItem = null;
        comboBox.SelectedValue = null;
        e.Handled = true;
        MessageBox.Show($"{inputText} - {strInfo}: ro'yxatda yo'q.", "Tekshiruv");
        try { comboBox.IsDropDownOpen = true; } catch { }
        return false;
    }
}

