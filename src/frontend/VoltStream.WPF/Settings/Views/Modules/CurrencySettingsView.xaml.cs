namespace VoltStream.WPF.Settings.Views.Modules;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VoltStream.WPF.Commons.ViewModels;
using VoltStream.WPF.Settings.ViewModels;

public partial class CurrencySettingsView : UserControl
{
    public CurrencySettingsView()
    {
        InitializeComponent();
    }

    private CurrencyViewModel? _draggedItem;

    private void DragHandle_MouseDown(object sender, MouseButtonEventArgs e)
    {
        var element = sender as FrameworkElement;
        _draggedItem = (element?.DataContext as CurrencyViewModel)!;
    }

    private void DragHandle_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Pressed && _draggedItem != null)
        {
            DragDrop.DoDragDrop((DependencyObject)sender, _draggedItem, DragDropEffects.Move);
        }
    }

    private void DragHandle_MouseUp(object sender, MouseButtonEventArgs e)
    {
        _draggedItem = null!;
    }

    private void ItemsControl_Drop(object sender, DragEventArgs e)
    {
        if (DataContext is not SettingsPageViewModel vm) return;

        if (e.Data.GetData(typeof(CurrencyViewModel)) is CurrencyViewModel draggedItem && ((FrameworkElement)e.OriginalSource).DataContext is CurrencyViewModel targetItem && draggedItem != targetItem)
        {
            var list = vm.Currencies;
            int oldIndex = list.IndexOf(draggedItem);
            int newIndex = list.IndexOf(targetItem);

            list.Move(oldIndex, newIndex);

            for (int i = 0; i < list.Count; i++)
            {
                list[i].Position = i + 1;
            }
        }
    }
}
