using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace VoltStream.WPF.Commons.Utils
{
    public static class ComboBoxTabBehavior
    {
        public static readonly DependencyProperty EnableTabOnSelectProperty =
            DependencyProperty.RegisterAttached(
                "EnableTabOnSelect",
                typeof(bool),
                typeof(ComboBoxTabBehavior),
                new UIPropertyMetadata(false, OnEnableTabOnSelectChanged));

        public static bool GetEnableTabOnSelect(DependencyObject obj)
            => (bool)obj.GetValue(EnableTabOnSelectProperty);

        public static void SetEnableTabOnSelect(DependencyObject obj, bool value)
            => obj.SetValue(EnableTabOnSelectProperty, value);

        private static void OnEnableTabOnSelectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ComboBox comboBox)
            {
                if ((bool)e.NewValue)
                {
                    comboBox.DropDownClosed += ComboBox_DropDownClosed;
                    comboBox.PreviewKeyDown += ComboBox_PreviewKeyDown;
                }
                else
                {
                    comboBox.DropDownClosed -= ComboBox_DropDownClosed;
                    comboBox.PreviewKeyDown -= ComboBox_PreviewKeyDown;
                }
            }
        }

        private static string? _lastText;
        private static void ComboBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is ComboBox comboBox && e.Key == Key.Enter && comboBox.IsDropDownOpen)
            {
                // Сохраняем текст до закрытия
                _lastText = comboBox.Text;
                // Закрываем дропдаун, после чего сработает DropDownClosed
                comboBox.IsDropDownOpen = false;
                e.Handled = true;
            }
        }

        private static void ComboBox_DropDownClosed(object? sender, System.EventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                //MessageBox.Show(comboBox.Text + " 1");
                SendTabKey();
                //// Проверяем, изменился ли текст после выбора
                //if (_lastText != null && comboBox.Text != _lastText)
                //{
                //    MessageBox.Show(comboBox.Text + " 2");
                //    _lastText = null;
                //    SendTabKey();
                //}
                //else if (Mouse.LeftButton == MouseButtonState.Pressed)
                //{
                //    MessageBox.Show(comboBox.Text + " 3");
                //    // Если выбрано мышкой, тоже переходим
                //    SendTabKey();
                //}
            }
        }

        private static void SendTabKey()
        {
            var tabDown = new KeyEventArgs(
                Keyboard.PrimaryDevice,
                PresentationSource.FromVisual(Application.Current.MainWindow),
                0,
                Key.Tab)
            {
                RoutedEvent = Keyboard.KeyDownEvent
            };
            InputManager.Current.ProcessInput(tabDown);

            var tabUp = new KeyEventArgs(
                Keyboard.PrimaryDevice,
                PresentationSource.FromVisual(Application.Current.MainWindow),
                0,
                Key.Tab)
            {
                RoutedEvent = Keyboard.KeyUpEvent
            };
            InputManager.Current.ProcessInput(tabUp);
        }
    }
}