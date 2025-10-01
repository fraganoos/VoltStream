namespace VoltStream.ServerManager;

using System.Windows;
using VoltStream.WebApi.Extensions;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private readonly ServerHostService _serverHost = new();

    public MainWindow()
    {
        InitializeComponent();
        ServerHostService.RequestReceived += OnRequestReceived;
    }

    private void OnRequestReceived(object? sender, RequestLog e)
    {
        Dispatcher.Invoke(() =>
        {
            LogsListBox.Items.Insert(0,
                $"{e.TimeStamp:HH:mm:ss} | {e.IpAddress} | {e.Method} {e.Path} | {e.UserAgent}");
        });
    }

    private async void StartButton_Click(object sender, RoutedEventArgs e)
    {
        await _serverHost.StartAsync();
    }

    private async void StopButton_Click(object sender, RoutedEventArgs e)
    {
        await _serverHost.StopAsync();
    }

    private async void RestartButton_Click(object sender, RoutedEventArgs e)
    {
        await _serverHost.RestartAsync();
    }
}
