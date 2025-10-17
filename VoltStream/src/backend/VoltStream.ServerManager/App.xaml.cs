namespace VoltStream.ServerManager;

using Microsoft.Extensions.Configuration;
using System.IO;
using System.Windows;
using VoltStream.ServerManager.Api;
using VoltStream.ServerManager.Enums;
using VoltStream.ServerManager.Services;

public partial class App : Application
{
    public static IAllowedClientsApi AllowedClientsApi { get; set; } = default!;
    public static ServerHostService ServerHost { get; private set; } = new();

    private NotifyIcon? trayIcon;
    private Window? settingsWindow;
    private Window? ipControlsWindow;
    private Window? logsWindow;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        StartupHelper.RegisterInStartup();

        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .Build();

        var port = config.GetValue("ServerPort", 5000);
        var host = config.GetValue("DatabaseHost", "localhost");

        var baseUrl = $"http://{host}:{port}/api";
        AllowedClientsApi = ApiFactory.CreateAllowedClients(baseUrl);

        ShutdownMode = ShutdownMode.OnExplicitShutdown;

        trayIcon = new NotifyIcon
        {
            Icon = GetStatusTrayIcon(),
            Visible = true,
            Text = "VoltStream Server Manager"
        };
        trayIcon.MouseClick += TrayIcon_MouseClick;

        // Server status o‘zgarganda tray yangilansin
        ServerHost.StatusChanged += (_, __) =>
        {
            if (Dispatcher.CheckAccess())
            {
                RefreshTray();
            }
            else
            {
                Dispatcher.Invoke(RefreshTray);
            }
        };

        RefreshTray();
        await ServerHost.StartAsync();
    }

    private void TrayIcon_MouseClick(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Right)
        {
            trayIcon!.ContextMenuStrip?.Show(Cursor.Position);
        }
        else if (e.Button == MouseButtons.Left)
        {
            ShowMainWindow();
        }
    }

    private void RefreshTray()
    {
        UpdateTrayMenu();
        UpdateTrayIcon();
    }

    private void UpdateTrayMenu()
    {
        var menu = new ContextMenuStrip();

        switch (ServerHost.Status)
        {
            case ServerStatus.Stopped:
                menu.Items.Add("Start", null, async (_, __) => await ServerHost.StartAsync());
                break;

            case ServerStatus.Starting:
                menu.Items.Add("Starting...").Enabled = false;
                break;

            case ServerStatus.Running:
                menu.Items.Add("Stop", null, async (_, __) => await ServerHost.StopAsync());
                menu.Items.Add("Restart", null, async (_, __) => await ServerHost.RestartAsync());
                break;

            case ServerStatus.Stopping:
                menu.Items.Add("Stopping...").Enabled = false;
                break;
        }

        menu.Items.Add("Settings", null, (_, __) => ShowSettingsWindow());
        menu.Items.Add("IP Controls", null, (_, __) => ShowIpControlsWindow());
        menu.Items.Add("Logs", null, (_, __) => ShowLogsWindow());
        menu.Items.Add("Quit", null, async (_, __) => await ExitAppAsync());

        trayIcon!.ContextMenuStrip = menu;
    }

    private async Task ExitAppAsync()
    {
        trayIcon!.Visible = false;
        try { await ServerHost.StopAsync(); } catch { }
        Shutdown();
    }

    private void UpdateTrayIcon()
    {
        if (trayIcon is null) return;
        trayIcon.Icon = GetStatusTrayIcon();
    }

    private static Icon GetStatusTrayIcon()
    {
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var iconPath = Path.Combine(baseDir, "Assets", "voltstream.ico");
        using var baseIcon = new Icon(iconPath);

        using var bmp = baseIcon.ToBitmap();
        using var g = Graphics.FromImage(bmp);

        int size = 10;
        int margin = 2;
        int x = margin;
        int y = margin;

        Color color = ServerHost.Status switch
        {
            ServerStatus.Stopped => Color.Red,
            ServerStatus.Starting => Color.Yellow,
            ServerStatus.Running => Color.LimeGreen,
            ServerStatus.Stopping => Color.Orange,
            _ => Color.Gray
        };

        using var brush = new SolidBrush(color);
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        g.FillEllipse(brush, x, y, size, size);

        return Icon.FromHandle(bmp.GetHicon());
    }

    #region Windows
    private void ShowMainWindow()
    {
        if (Current.MainWindow is null || !Current.MainWindow.IsVisible)
        {
            TryCloseAuxWindows();
            Current.MainWindow = new MainWindow();
            Current.MainWindow.Show();
        }
        else
        {
            if (Current.MainWindow.WindowState == WindowState.Minimized)
                Current.MainWindow.WindowState = WindowState.Normal;

            Current.MainWindow.Activate();
        }
    }

    private void ShowSettingsWindow()
    {
        if (settingsWindow is { IsVisible: true })
        {
            BringToFront(settingsWindow);
            return;
        }
        settingsWindow = new Window
        {
            Title = "Settings",
            Content = new Pages.Settings.SettingsPage(),
            Width = 600,
            Height = 400
        };
        settingsWindow.Closed += (_, __) => settingsWindow = null;
        settingsWindow.Show();
    }

    private void ShowIpControlsWindow()
    {
        if (ipControlsWindow is { IsVisible: true })
        {
            BringToFront(ipControlsWindow);
            return;
        }
        ipControlsWindow = new Window
        {
            Title = "IP Controls",
            Content = new Pages.IPAccessPage(AllowedClientsApi),
            Width = 600,
            Height = 400
        };
        ipControlsWindow.Closed += (_, __) => ipControlsWindow = null;
        ipControlsWindow.Show();
    }

    private void ShowLogsWindow()
    {
        if (logsWindow is { IsVisible: true })
        {
            BringToFront(logsWindow);
            return;
        }
        logsWindow = new Window
        {
            Title = "Logs",
            Content = new Pages.DashboardPage(ServerHost, null!),
            Width = 800,
            Height = 500
        };
        logsWindow.Closed += (_, __) => logsWindow = null;
        logsWindow.Show();
    }

    private static void BringToFront(Window window)
    {
        if (window.WindowState == WindowState.Minimized)
            window.WindowState = WindowState.Normal;

        window.Activate();
        window.Topmost = true;
        window.Topmost = false;
        window.Focus();
    }

    private void TryCloseAuxWindows()
    {
        settingsWindow?.Close();
        settingsWindow = null;

        ipControlsWindow?.Close();
        ipControlsWindow = null;

        logsWindow?.Close();
        logsWindow = null;
    }
    #endregion
}
