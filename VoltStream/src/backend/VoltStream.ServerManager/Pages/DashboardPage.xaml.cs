namespace VoltStream.ServerManager.Pages;

using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using VoltStream.ServerManager.Enums;
using VoltStream.WebApi.Extensions;

public partial class DashboardPage : Page
{
    private readonly ServerHostService serverHost;
    private readonly Ellipse? statusIndicator;

    public DashboardPage(ServerHostService serverHost, Ellipse? statusIndicator)
    {
        InitializeComponent();
        this.serverHost = serverHost;
        this.statusIndicator = statusIndicator;

        foreach (var log in serverHost.Logs)
        {
            AddLogToList(log);
        }

        this.serverHost.StatusChanged += OnServerStatusChanged;
        this.serverHost.RequestReceived += OnRequestReceived;

        // Initialize indicator immediately based on current state
        var initialStatus = serverHost.IsRunning ? ServerStatus.Running : ServerStatus.Stopped;
        SetIndicator(initialStatus);

        // Unsubscribe on unload to avoid callbacks after page/window closed
        this.Unloaded += (_, __) =>
        {
            this.serverHost.StatusChanged -= OnServerStatusChanged;
            this.serverHost.RequestReceived -= OnRequestReceived;
        };
    }

    private void OnRequestReceived(object? sender, RequestLog log)
    {
        Dispatcher.Invoke(() =>
        {
            AddLogToList(log);
        });
    }

    private void OnServerStatusChanged(object? sender, ServerStatus status)
    {
        Dispatcher.Invoke(() => SetIndicator(status));
    }

    private void SetIndicator(ServerStatus status)
    {
        if (statusIndicator is null) return;
        statusIndicator.Fill = status switch
        {
            ServerStatus.Running => Brushes.Green,
            ServerStatus.Stopped => Brushes.Red,
            ServerStatus.Starting => Brushes.Yellow,
            ServerStatus.Stopping => Brushes.Orange,
            _ => Brushes.Gray
        };
    }

    private void AddLogToList(RequestLog log)
    {
        LogsListBox.Items.Insert(0,
            $"{(log.IsSuccess ? "⚡" : "❌")} {log.TimeStamp:HH:mm:ss} | " +
            $"{log.IpAddress} | {log.Method} {log.Path} | " +
            $"{log.UserAgent} | {log.StatusCode}");
    }

    private async void StartButton_Click(object sender, System.Windows.RoutedEventArgs e)
        => await serverHost.StartAsync();

    private async void StopButton_Click(object sender, System.Windows.RoutedEventArgs e)
        => await serverHost.StopAsync();

    private async void RestartButton_Click(object sender, System.Windows.RoutedEventArgs e)
        => await serverHost.RestartAsync();
}
