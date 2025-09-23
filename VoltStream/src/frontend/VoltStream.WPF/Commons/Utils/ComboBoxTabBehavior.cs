using System;
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

        public static readonly DependencyProperty NextKeyProperty =
            DependencyProperty.RegisterAttached(
                "NextKey",
                typeof(Key),
                typeof(ComboBoxTabBehavior),
                new UIPropertyMetadata(Key.Tab));

        public static bool GetEnableTabOnSelect(DependencyObject obj) =>
            (bool)obj.GetValue(EnableTabOnSelectProperty);

        public static void SetEnableTabOnSelect(DependencyObject obj, bool value) =>
            obj.SetValue(EnableTabOnSelectProperty, value);

        public static Key GetNextKey(DependencyObject obj) =>
            (Key)obj.GetValue(NextKeyProperty);

        public static void SetNextKey(DependencyObject obj, Key value) =>
            obj.SetValue(NextKeyProperty, value);

        private static void OnEnableTabOnSelectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not ComboBox comboBox)
                return;

            if ((bool)e.NewValue)
            {
                comboBox.DropDownClosed += ComboBox_DropDownClosed;
                comboBox.PreviewKeyDown += ComboBox_PreviewKeyDown;
                comboBox.PreviewMouseDown += ComboBox_PreviewMouseDown;
                comboBox.Unloaded += ComboBox_Unloaded;

                SetComboBoxState(comboBox, new State());
            }
            else
            {
                comboBox.DropDownClosed -= ComboBox_DropDownClosed;
                comboBox.PreviewKeyDown -= ComboBox_PreviewKeyDown;
                comboBox.PreviewMouseDown -= ComboBox_PreviewMouseDown;
                comboBox.Unloaded -= ComboBox_Unloaded;

                ClearComboBoxState(comboBox);
            }
        }

        #region State per ComboBox
        private class State
        {
            public bool MouseClicked { get; set; }
            public bool EnterPressed { get; set; }
        }

        private static readonly DependencyProperty StateProperty =
            DependencyProperty.RegisterAttached(
                "State",
                typeof(State),
                typeof(ComboBoxTabBehavior),
                new PropertyMetadata(null));

        private static void SetComboBoxState(ComboBox comboBox, State state) =>
            comboBox.SetValue(StateProperty, state);

        private static State? GetComboBoxState(ComboBox comboBox) =>
            comboBox.GetValue(StateProperty) as State;

        private static void ClearComboBoxState(ComboBox comboBox) =>
            comboBox.ClearValue(StateProperty);
        #endregion

        private static void ComboBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is ComboBox comboBox && comboBox.IsDropDownOpen)
            {
                var state = GetComboBoxState(comboBox);
                if (state != null) state.MouseClicked = true;
            }
        }

        private static void ComboBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is not ComboBox comboBox)
                return;

            var state = GetComboBoxState(comboBox);
            if (state == null) return;

            // ENTER → переход вперёд (обычно Tab)
            if (e.Key == Key.Enter)
            {
                state.EnterPressed = true;

                if (!comboBox.IsDropDownOpen)
                {
                    e.Handled = true;
                    DoNavigationForKey(comboBox, GetNextKey(comboBox));
                }
                else
                {
                    comboBox.IsDropDownOpen = false;
                }

                return;
            }

            // стрелки ← / ↑ → назад
            if (e.Key == Key.Left || e.Key == Key.Up)
            {
                if (TryGetEditableTextBox(comboBox, out var textBox))
                {
                    if (textBox.CaretIndex == 0 || string.IsNullOrEmpty(textBox.Text))
                    {
                        e.Handled = true;

                        if (comboBox.IsDropDownOpen)
                            comboBox.IsDropDownOpen = false;

                        MoveFocusPrevious(comboBox);
                    }
                }
            }

            // стрелки → / ↓ → вперёд
            if (e.Key == Key.Right || e.Key == Key.Down)
            {
                if (TryGetEditableTextBox(comboBox, out var textBox))
                {
                    // если курсор в конце текста → идём дальше
                    if (textBox.CaretIndex == textBox.Text.Length)
                    {
                        e.Handled = true;

                        if (comboBox.IsDropDownOpen)
                            comboBox.IsDropDownOpen = false;

                        DoNavigationForKey(comboBox, Key.Tab);
                    }
                }
            }
        }

        private static void ComboBox_DropDownClosed(object? sender, EventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                var state = GetComboBoxState(comboBox);
                if (state == null) return;

                if (state.EnterPressed)
                {
                    state.EnterPressed = false;
                    DoNavigationForKey(comboBox, GetNextKey(comboBox));
                    return;
                }

                if (state.MouseClicked)
                {
                    state.MouseClicked = false;
                    DoNavigationForKey(comboBox, GetNextKey(comboBox));
                }
            }
        }

        private static void ComboBox_Unloaded(object sender, RoutedEventArgs e)
        {
            if (sender is ComboBox comboBox)
                ClearComboBoxState(comboBox);
        }

        private static bool TryGetEditableTextBox(ComboBox comboBox, out TextBox textBox)
        {
            textBox = comboBox.Template.FindName("PART_EditableTextBox", comboBox) as TextBox;
            return textBox != null;
        }

        // Навигация через MoveFocus — корректно имитирует Tab / Shift+Tab
        private static void DoNavigationForKey(ComboBox comboBox, Key key)
        {
            var focused = Keyboard.FocusedElement as UIElement ?? comboBox;
            if (focused == null) return;

            if (key == Key.Tab || key == Key.Enter || key == Key.Right || key == Key.Down)
            {
                focused.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
            else if (key == Key.Left || key == Key.Up)
            {
                focused.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
            }
        }

        private static void MoveFocusPrevious(ComboBox comboBox)
        {
            var focused = Keyboard.FocusedElement as UIElement ?? comboBox;
            if (focused != null)
                focused.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
        }
    }
}
