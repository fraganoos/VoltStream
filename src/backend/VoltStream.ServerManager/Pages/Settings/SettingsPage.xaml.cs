namespace VoltStream.ServerManager.Pages.Settings;

using System.Windows;
using System.Windows.Controls;

public partial class SettingsPage : Page
{
    public SettingsPage()
    {
        InitializeComponent();
        DataContext = new SettingsViewModel();
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is SettingsViewModel vm)
        {
            vm.Settings.ConnectionStrings.Password = ((PasswordBox)sender).Password;
        }
    }
}
