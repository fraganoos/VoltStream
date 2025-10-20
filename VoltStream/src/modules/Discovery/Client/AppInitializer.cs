namespace Discovery.Client;

using Discovery.Core;
using Discovery.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Diagnostics;

public class AppInitializer(
    DiscoveryClient discoveryClient,
    ILogger<AppInitializer> logger,
    IOptions<DiscoveryOptions> options)
{
    public async Task InitializeAsync()
    {
        var config = options.Value;
        var baseUrl = $"http://{config.Host}:{config.ServerPort}";

        logger.LogInformation("🔍 Checking server at {url}", baseUrl);

        using var http = new HttpClient { BaseAddress = new Uri(baseUrl) };

        try
        {
            var response = await http.GetAsync("/health");
            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("✅ Server is available.");
                return;
            }
        }
        catch
        {
            logger.LogWarning("⚠️ Initial connection failed. Starting discovery...");
        }

        var found = await discoveryClient.DiscoverServerAsync();
        if (found is null)
        {
            logger.LogError("❌ No server discovered.");
            return;
        }

        var discoveredUrl = $"http://{found.Address}:{found.Port}/api";
        logger.LogInformation("💾 Discovered server: {url}", discoveredUrl);

        // ✅ Update config
        AppSettingsHelper.UpdateServerConfig("Discovery", found.Address.ToString(), found.Port);

        // ✅ Update ApiBaseUrl
        AppSettingsHelper.UpdateApiBaseUrl(discoveredUrl);

        // ✅ Restart
        RestartApplication();
    }

    private void RestartApplication()
    {
        logger.LogInformation("🔁 Restarting application...");
        Process.Start(Environment.ProcessPath!);
        Environment.Exit(0);
    }
}