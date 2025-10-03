namespace VoltStream.ServerManager;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net.Http;
using VoltStream.ServerManager.Enums;
using VoltStream.WebApi;
using VoltStream.WebApi.Extensions;

public class ServerHostService
{
    private WebApplication? app;
    private CancellationTokenSource? cts;
    private readonly List<RequestLog> logs = [];

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

        var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var port = config.GetValue("Server:Port", 5000);
        var scheme = config.GetValue("Server:UseHttps", false) ? "https" : "http";

        app = WebApiHostBuilder.Build([], config, log =>
        {
            if (log.Path!.StartsWith("/api"))
            {
                logs.Add(log);
                RequestReceived?.Invoke(this, log);
            }
        });

        app.Urls.Clear();
        app.Urls.Add($"{scheme}://0.0.0.0:{port}");

        cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        await app.StartAsync(cts.Token);

        var realUrl = $"{scheme}://localhost:{port}";
        var pathQuery = "/scalar/v1";

        var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
        lifetime.ApplicationStarted.Register(async () =>
        {
            try
            {
                using var http = new HttpClient();
                http.Timeout = TimeSpan.FromSeconds(5);

                var response = await http.GetAsync(realUrl + pathQuery);

                var log = new RequestLog
                {
                    TimeStamp = DateTime.Now,
                    IpAddress = "127.0.0.1",
                    Method = "GET",
                    Path = pathQuery,
                    UserAgent = "ServerMonitor",
                    StatusCode = (int)response.StatusCode,
                    IsSuccess = response.IsSuccessStatusCode
                };

                ForwardLog(log);

                Status = response.IsSuccessStatusCode
                    ? ServerStatus.Running
                    : ServerStatus.Stopped;
            }
            catch
            {
                Status = ServerStatus.Stopped;
            }
        });
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
    }

    private void ForwardLog(RequestLog log)
    {
        logs.Add(log);
        RequestReceived?.Invoke(this, log);
    }
}
