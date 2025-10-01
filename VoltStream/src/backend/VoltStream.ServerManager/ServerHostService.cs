namespace VoltStream.ServerManager;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using VoltStream.WebApi;
using VoltStream.WebApi.Extensions;

public class ServerHostService
{
    public static event EventHandler<RequestLog>? RequestReceived;

    private WebApplication? app;
    private CancellationTokenSource? cts;

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        if (app != null)
            return;

        var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        app = WebApiHostBuilder.Build(Array.Empty<string>(), config);

        cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        // request loglarni WPF ga forward qilamiz
        app.UseRequestLogging(log =>
        {
            RequestReceived?.Invoke(this, log);
        });

        _ = app.RunAsync(cts.Token);

        await Task.Delay(500, cancellationToken);
    }

    public async Task StopAsync(CancellationToken cancellationToken = default)
    {
        if (app is null)
            return;

        try
        {
            cts?.Cancel();
            await app.StopAsync(cancellationToken);
        }
        finally
        {
            app = null;
            cts = null;
        }
    }

    public async Task RestartAsync(CancellationToken cancellationToken = default)
    {
        await StopAsync(cancellationToken);
        await StartAsync(cancellationToken);
    }
}
