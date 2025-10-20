namespace VoltStream.WPF.Configurations;

using ApiServices.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class AppInitializer(IHostEnvironment env)
{
    public async Task InitializeAsync()
    {
        var uri = await DiscoveryClient.DiscoverAsync();
        if (uri is null)
            return;

        var host = env.IsDevelopment() ? "localhost" : uri.Host;
        var apiUrl = $"{uri.Scheme}://{host}:{uri.Port}/api";

        Console.WriteLine($"✅ Final API: {apiUrl}");

        var urlHolder = App.Services!.GetRequiredService<ApiUrlHolder>();
        urlHolder.Url = apiUrl;
    }
}
