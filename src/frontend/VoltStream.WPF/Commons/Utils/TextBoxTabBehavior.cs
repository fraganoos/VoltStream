namespace VoltStream.WPF.Commons.Utils;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;


public static class TextBoxTabBehavior
{
    public static readonly DependencyProperty EnableTabOnKeysProperty =
        DependencyProperty.RegisterAttached(
            "EnableTabOnKeys",
            typeof(bool),
            typeof(TextBoxTabBehavior),
            new UIPropertyMetadata(false, OnEnableTabOnKeysChanged));

    public static bool GetEnableTabOnKeys(DependencyObject obj) =>
        (bool)obj.GetValue(EnableTabOnKeysProperty);

    public static void SetEnableTabOnKeys(DependencyObject obj, bool value) =>
        obj.SetValue(EnableTabOnKeysProperty, value);

    private static void OnEnableTabOnKeysChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not TextBox textBox)
            return;

        if ((bool)e.NewValue)
        {
            textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
            textBox.Unloaded += TextBox_Unloaded;
        }
        else
        {
            textBox.PreviewKeyDown -= TextBox_PreviewKeyDown;
            textBox.Unloaded -= TextBox_Unloaded;
        }
    }

    private static void TextBox_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (sender is not TextBox textBox)
            return;

        bool allSelected = textBox.SelectionStart == 0 &&
                           textBox.SelectionLength == textBox.Text.Length &&
                           textBox.Text.Length > 0;

        // ENTER → Tab (вперёд)
        if (e.Key == Key.Enter)
        {
            e.Handled = true;
            MoveFocusNext(textBox);
            return;
        }

        // ← / ↑ → Shift+Tab (назад), если в начале текста или всё выделено
        if (e.Key == Key.Left || e.Key == Key.Up)
        {
            if (textBox.CaretIndex == 0 || string.IsNullOrEmpty(textBox.Text) || allSelected)
            {
                e.Handled = true;
                MoveFocusPrevious(textBox);
            }
        }

        // → / ↓ → Tab (вперёд), если в конце текста или всё выделено
        if (e.Key == Key.Right || e.Key == Key.Down)
        {
            if (textBox.CaretIndex == textBox.Text.Length || allSelected)
            {
                e.Handled = true;
                MoveFocusNext(textBox);
            }
        }
    }

    private static void TextBox_Unloaded(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            textBox.PreviewKeyDown -= TextBox_PreviewKeyDown;
            textBox.Unloaded -= TextBox_Unloaded;
        }
    }

    private static void MoveFocusNext(Control control) =>
        (Keyboard.FocusedElement as UIElement ?? control)?
            .MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));

    private static void MoveFocusPrevious(Control control) =>
        (Keyboard.FocusedElement as UIElement ?? control)?
            .MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
}
