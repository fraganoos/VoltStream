namespace VoltStream.WPF.Settings.Views;

using System.Windows;
using System.Windows.Input;
using VoltStream.WPF.Settings.ViewModels;

public partial class ConnectionSettingsWindow : Window
{
    public ConnectionSettingsWindow(ConnectionSettingsViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;

        // Oynani yopish actionini o'rnatish
        vm.SetCloseAction(() =>
        {
            DialogResult = true;
            Close();
        });

        // Focus ni Host TextBox ga o'rnatish
        Loaded += (s, e) => HostTextBox.Focus();
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }

    // Oynani sichqoncha bilan sudrab yurish
    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);
        if (e.ButtonState == MouseButtonState.Pressed)
            DragMove();
    }
}