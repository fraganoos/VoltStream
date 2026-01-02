namespace VoltStream.WPF.Commons.Utils;

using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

public static class ComboBoxHelper
{
    public static bool BeforeUpdate(object sender, KeyboardFocusChangedEventArgs e, string? strInfo, bool allow = false)
    {
        if (sender is not ComboBox cb) return false;

        var input = cb.Text?.Trim() ?? "";
        if (string.IsNullOrEmpty(input)) return false;

        var items = cb.ItemsSource?.Cast<object>() ?? [];
        var prop = string.IsNullOrEmpty(cb.DisplayMemberPath) ? null : items.FirstOrDefault()?.GetType().GetProperty(cb.DisplayMemberPath);

        bool exists = items.Any(item =>
        {
            var val = prop != null ? prop.GetValue(item)?.ToString() : item?.ToString();
            return string.Equals(val?.Trim(), input, StringComparison.OrdinalIgnoreCase);
        });

        if (exists) return false;

        if (allow && MessageBox.Show($"{input} - {strInfo}: ro'yxatda yo'q. Yangi qo'shilsinmi?", "Tekshiruv", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            return true;

        cb.Text = "";
        cb.SelectedItem = cb.SelectedValue = null;
        e.Handled = true;

        if (!allow) MessageBox.Show($"{input} - {strInfo}: ro'yxatda yo'q.", "Tekshiruv", MessageBoxButton.OK, MessageBoxImage.Warning);

        Application.Current.Dispatcher.BeginInvoke(() => { try { cb.IsDropDownOpen = true; } catch { } });
        return false;
    }
}