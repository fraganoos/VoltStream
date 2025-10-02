namespace VoltStream.ServerManager;

using System.Windows;
using VoltStream.ServerManager.Pages;
using VoltStream.ServerManager.Pages.Settings;

public partial class MainWindow : Window
{
    private readonly ServerHostService serverHost = new();

    public MainWindow()
    {
        InitializeComponent();
        MainFrame.Navigate(new DashboardPage(serverHost, ServerStatusIndicator));
    }

    private void DashboardButton_Click(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(new DashboardPage(serverHost, ServerStatusIndicator));
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(new SettingsPage());
    }

    private void IpAccessButton_Click(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(new IPAccessPage(App.AllowedClientsApi));
    }
}