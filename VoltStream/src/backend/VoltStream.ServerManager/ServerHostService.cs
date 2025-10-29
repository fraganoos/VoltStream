namespace VoltStream.ServerManager;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Net.Http;
using VoltStream.ServerManager.Api;
using VoltStream.ServerManager.Enums;
using VoltStream.WebApi;
using VoltStream.WebApi.Models;

public class ServerHostService
{
    private WebApplication? app;
    private CancellationTokenSource? cts;

    private readonly List<RequestLog> logs = [];
    private static readonly HttpClient http = new() { Timeout = TimeSpan.FromSeconds(5) };

    private ServerStatus status = ServerStatus.Stopped;
    public ServerStatus Status
    {
        get => status;
        private set
        {
            if (status != value)
            {
                status = value;
                StatusChanged?.Invoke(this, status);
            }
        }
    }

    public bool IsRunning => Status == ServerStatus.Running;

    public event EventHandler<RequestLog>? RequestReceived;
    public event EventHandler<ServerStatus>? StatusChanged;

    public IReadOnlyList<RequestLog> Logs => logs;

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (app is not null) return;

        Status = ServerStatus.Starting;

        var config = LoadConfiguration();
        var port = config.GetValue("Server:Port", 5000);
        var scheme = config.GetValue("Server:UseHttps", false) ? "https" : "http";

        app = WebApiHostBuilder.Build(
            args: [],
            externalConfig: config,
            logCallback: log =>
            {
                logs.Add(log);
                RequestReceived?.Invoke(this, log);
            });

        app.Urls.Clear();
        app.Urls.Add($"{scheme}://0.0.0.0:{port}");

        cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        await app.StartAsync(cts.Token);

        await CheckHealthAsync(scheme, port);
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (app is null) return;

        Status = ServerStatus.Stopping;
        cts?.Cancel();
        await app.StopAsync(cancellationToken);
        app = null;
        cts = null;
        Status = ServerStatus.Stopped;
    }

    public async Task RestartAsync(CancellationToken cancellationToken = default)
    {
        await StopAsync(cancellationToken);
        await StartAsync(cancellationToken);

        var config = LoadConfiguration();
        var port = config.GetValue("Server:Port", 5000);
        var host = config.GetValue("Server:Host", "localhost");
        var scheme = config.GetValue("Server:UseHttps", false) ? "https" : "http";

        var baseUrl = $"{scheme}://{host}:{port}/api";
        App.AllowedClientsApi = ApiFactory.CreateAllowedClients(baseUrl);
    }

    private async Task CheckHealthAsync(string scheme, int port)
    {
        var url = $"{scheme}://localhost:{port}/api/health";
        var sw = Stopwatch.StartNew();

        try
        {
            var response = await http.GetAsync(url);
            sw.Stop();

            var log = new RequestLog(
                TimeStamp: DateTime.UtcNow,
                IpAddress: "127.0.0.1",
                Method: "GET",
                Path: "/api/health",
                UserAgent: "ServerMonitor",
                StatusCode: (int)response.StatusCode,
                IsSuccess: response.IsSuccessStatusCode,
                ElapsedMs: sw.ElapsedMilliseconds
            );

            logs.Add(log);
            RequestReceived?.Invoke(this, log);

            Status = response.IsSuccessStatusCode
                ? ServerStatus.Running
                : ServerStatus.Stopped;
        }
        catch
        {
            sw.Stop();

            var log = new RequestLog(
                TimeStamp: DateTime.UtcNow,
                IpAddress: "127.0.0.1",
                Method: "GET",
                Path: "/scalar/v1",
                UserAgent: "ServerMonitor",
                StatusCode: 500,
                IsSuccess: false,
                ElapsedMs: sw.ElapsedMilliseconds
            );

            logs.Add(log);
            RequestReceived?.Invoke(this, log);

            Status = ServerStatus.Stopped;
        }
    }


    private static IConfiguration LoadConfiguration() =>
        new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();
}
