namespace VoltStream.WPF.Configurations;

using ApiServices.Extensions;
using ApiServices.Interfaces;
using ApiServices.Services;
using Microsoft.Extensions.Hosting;

public class ConnectionMonitor(
    ApiUrlHolder urlHolder,
    IHostEnvironment env,
    IHealthCheckApi client)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var response = await client.GetAsync(stoppingToken).Handle();
            if (!response.IsSuccess)
                await TryRediscoverAsync();
            await Task.Delay(5000, stoppingToken);
        }
    }

    private async Task TryRediscoverAsync()
    {
        while (true)
        {
            var uri = await DiscoveryClient.DiscoverAsync();
            if (uri is not null)
            {
                var host = env.IsDevelopment() ? "localhost" : uri.Host;
                var newUrl = $"{uri.Scheme}://{host}:{uri.Port}/api";

                if (urlHolder.Url != newUrl)
                    urlHolder.Url = newUrl;

                return; // ✅ Rediscovery successful, exit loop
            }

            await Task.Delay(5000);
        }
    }
}
