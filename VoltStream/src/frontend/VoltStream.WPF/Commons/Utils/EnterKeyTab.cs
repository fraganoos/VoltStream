namespace VoltStream.WPF.Commons.Utils;

using System.Windows;
using System.Windows.Input;

public static class EnterKeyTab
{
    public static readonly DependencyProperty IsEnabledProperty =
        DependencyProperty.RegisterAttached(
            "IsEnabled",
            typeof(bool),
            typeof(EnterKeyTab),
            new UIPropertyMetadata(false, OnIsEnabledChanged));

    public static bool GetIsEnabled(DependencyObject obj)
    {
        return (bool)obj.GetValue(IsEnabledProperty);
    }

    public static void SetIsEnabled(DependencyObject obj, bool value)
    {
        obj.SetValue(IsEnabledProperty, value);
    }

    private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is UIElement element)
        {
            if ((bool)e.NewValue)
            {
                element.PreviewKeyDown += Element_PreviewKeyDown;
            }
            else
            {
                element.PreviewKeyDown -= Element_PreviewKeyDown;
            }
        }
    }
    private static void Element_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
        {
            e.Handled = true; // предотвращаем стандартное поведение

            // Enter-ni Tab-ga aylantirish uchun yangi KeyEventArgs yaratamiz
            KeyEventArgs tabKeyEvent = new KeyEventArgs(
                Keyboard.PrimaryDevice,
                Keyboard.PrimaryDevice.ActiveSource,
                0,
                Key.Tab)
            {
                RoutedEvent = Keyboard.KeyDownEvent
            };

            // Отправляем событие текущему элементу
            InputManager.Current.ProcessInput(tabKeyEvent);
        }
    }
}

public static class FocusMovement
{
    // Метод для перевода фокуса по имени
    public static void MoveFocusToElement(string elementName, DependencyObject parent)
    {
        Window window = Window.GetWindow(parent);
        if (window != null)
        {
            if (window.FindName(elementName) is UIElement targetElement)
            {
                targetElement.Focus();
            }
        }
    }

}
