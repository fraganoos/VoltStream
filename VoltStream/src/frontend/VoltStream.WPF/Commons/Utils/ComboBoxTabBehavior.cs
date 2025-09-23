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
                    comboBox.PreviewMouseDown += ComboBox_PreviewMouseDown;
                }
                else
                {
                    comboBox.DropDownClosed -= ComboBox_DropDownClosed;
                    comboBox.PreviewKeyDown -= ComboBox_PreviewKeyDown;
                    comboBox.PreviewMouseDown -= ComboBox_PreviewMouseDown;
                }
            }
        }

        private static string? _lastText;
        private static bool _mouseClicked = false;
        private static void ComboBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            _mouseClicked = true;
        }

        private static void ComboBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is ComboBox comboBox && e.Key == Key.Enter )
            {
                // Сохраняем текст до закрытия
                _lastText = comboBox.Text;
                if (!comboBox.IsDropDownOpen)
                {
                    e.Handled = true; // Предотвращаем дальнейшую обработку Enter
                    SendTabKey();
                    return;
                }
                else 
                {
                    comboBox.IsDropDownOpen = false;
                    e.Handled = true;
                    SendTabKey();
                }
                // Закрываем дропдаун, после чего сработает DropDownClosed
            }
        }

        private static void ComboBox_DropDownClosed(object? sender, System.EventArgs e)
        {
            //if (sender is ComboBox comboBox)
            //{
            //    // Проверяем, изменился ли текст после выбора
            //    if (comboBox.Text != null) //(_lastText != null && comboBox.Text != _lastText)
            //    {
            //        //MessageBox.Show(comboBox.Text + " 2");
            //        _lastText = null;
            //        SendTabKey();
            //    }
            //    if (_mouseClicked) //(Mouse.LeftButton == MouseButtonState.Pressed)
            //    {
            //        // Если выбрано мышкой, тоже переходим
            //        _mouseClicked = false;
            //        SendTabKey();
            //    }
            //}
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