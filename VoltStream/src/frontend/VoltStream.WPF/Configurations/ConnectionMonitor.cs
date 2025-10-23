namespace VoltStream.WPF.Configurations;
using Microsoft.Extensions.Hosting;
using VoltStream.WPF.Commons.ViewModels;

public class ConnectionMonitor(
    ApiConnectionViewModel state,
    ConnectionTester tester)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            if (!state.CheckUrlEnabled && !state.AutoReconnectEnabled)
            {
                await Task.Delay(5000, stoppingToken);
                continue;
            }

            try
            {
                state.IsConnected = await tester.TestAsync();
            }
            catch (Exception) { state.IsConnected = false; }

            if (!state.IsConnected && state.AutoReconnectEnabled)
                await TryRediscoverAsync(stoppingToken);

            await Task.Delay(5000, stoppingToken);
        }
    }

    private async Task TryRediscoverAsync(CancellationToken token)
    {
        while (!token.IsCancellationRequested && state.AutoReconnectEnabled)
        {
            var uri = await DiscoveryClient.DiscoverAsync();
            if (uri is not null)
            {
                var newUrl = $"{uri.Scheme}://{uri.Host}:{uri.Port}/";
                if (state.Url != newUrl)
                    state.Url = newUrl;

                state.IsConnected = true;
                return;
            }

            await Task.Delay(5000, token);
        }
    }
}
