using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace VoltStream.WPF.Commons.Utils
{
    public static class ComboBoxQuickNavBehavior
    {
        public static readonly DependencyProperty EnableQuickNavigationProperty =
            DependencyProperty.RegisterAttached(
                "EnableQuickNavigation",
                typeof(bool),
                typeof(ComboBoxQuickNavBehavior),
                new PropertyMetadata(false, OnEnableChanged));

        public static void SetEnableQuickNavigation(DependencyObject obj, bool value)
            => obj.SetValue(EnableQuickNavigationProperty, value);

        public static bool GetEnableQuickNavigation(DependencyObject obj)
            => (bool)obj.GetValue(EnableQuickNavigationProperty);

        private static void OnEnableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ComboBox combo)
                return;

            if ((bool)e.NewValue)
            {
                combo.PreviewKeyDown += OnPreviewKeyDown;
                combo.DropDownClosed += OnDropDownClosed;
                combo.PreviewMouseDown += OnPreviewMouseDown;
                combo.GotFocus += OnGotFocus;
            }
            else
            {
                combo.PreviewKeyDown -= OnPreviewKeyDown;
                combo.DropDownClosed -= OnDropDownClosed;
                combo.PreviewMouseDown -= OnPreviewMouseDown;
                combo.GotFocus -= OnGotFocus;
            }
        }

        // флаг: был ли клик внутри списка
        private static bool _clickedInsideList;

        private static void OnGotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is ComboBox combo)
            {
                combo.Dispatcher.BeginInvoke(() =>
                {
                    if (!combo.IsDropDownOpen)
                        combo.IsDropDownOpen = true;
                }, DispatcherPriority.Background);
            }
        }

        private static void OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ComboBox combo)
            {
                _clickedInsideList = false;

                // Если кликаем по элементу списка
                if (combo.IsDropDownOpen && e.OriginalSource is FrameworkElement fe)
                {
                    // Проверяем, является ли клик частью списка
                    DependencyObject current = fe;
                    while (current != null)
                    {
                        if (current is ListBoxItem)
                        {
                            _clickedInsideList = true;
                            break;
                        }
                        current = VisualTreeHelper.GetParent(current);
                    }
                }
            }
        }

        private static void OnDropDownClosed(object sender, EventArgs e)
        {
            if (sender is not ComboBox combo)
                return;

            if (_clickedInsideList)
            {
                // если клик по элементу списка → переход фокуса
                combo.Dispatcher.BeginInvoke(() =>
                {
                    MoveFocus(combo, FocusNavigationDirection.Next);
                }, DispatcherPriority.Background);
            }

            _clickedInsideList = false;
        }

        private static void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is not ComboBox combo)
                return;

            var direction = FocusNavigationDirection.Next;
            bool handled = false;

            switch (e.Key)
            {
                case Key.Enter:
                case Key.Down:
                case Key.Right:
                    direction = FocusNavigationDirection.Next;
                    handled = true;
                    break;

                case Key.Up:
                case Key.Left:
                    direction = FocusNavigationDirection.Previous;
                    handled = true;
                    break;
            }

            if (handled)
            {
                e.Handled = true;
                combo.IsDropDownOpen = false;

                combo.Dispatcher.BeginInvoke(() =>
                {
                    MoveFocus(combo, direction);
                }, DispatcherPriority.Input);
            }
        }

        private static void MoveFocus(Control control, FocusNavigationDirection direction)
        {
            var focused = Keyboard.FocusedElement as UIElement ?? control;
            focused?.MoveFocus(new TraversalRequest(direction));
        }
    }
}
