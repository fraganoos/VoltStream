namespace Discovery.Extensions;

using Discovery.Abstractions;
using Discovery.Client;
using Discovery.Core;
using Discovery.Implementations;
using Discovery.Models;
using Discovery.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDiscoveryModule(this IServiceCollection services, IConfiguration configuration)
    {
        // Use configuration section and the Options configuration extension (requires Options.ConfigurationExtensions)
        var discoverySection = configuration.GetSection("Discovery");
        services.Configure<DiscoveryOptions>(discoverySection);

        // get cache path from section (requires Configuration.Binder for GetValue)
        var cacheFilePath = discoverySection.GetValue<string>("CacheFilePath") ?? "discovery.cache";
        services.AddSingleton(new DiscoveryCache(cacheFilePath));

        // register implementations
        services.AddSingleton<UdpBroadcastDiscovery>();
        services.AddSingleton<UdpMulticastDiscovery>();
        services.AddSingleton<IpScannerDiscovery>();

        // register DiscoveryManager and expose as IServerDiscovery
        services.AddSingleton<DiscoveryManager>();
        services.AddSingleton<IServerDiscovery>(sp =>
        {
            var list = new IServerDiscovery[]
            {
                sp.GetRequiredService<UdpBroadcastDiscovery>(),
                sp.GetRequiredService<UdpMulticastDiscovery>(),
                sp.GetRequiredService<IpScannerDiscovery>()
            };

            return new DiscoveryManager(
                list,
                sp.GetRequiredService<DiscoveryCache>(),
                sp.GetRequiredService<ILogger<DiscoveryManager>>()
            );
        });

        // responder service
        services.AddHostedService<DiscoveryResponderService>();

        return services;
    }

    public static IServiceCollection AddDiscoveryClient(this IServiceCollection services, IConfiguration configuration)
    {
        // ⚙️ Config (Options) – backend bilan mos bo‘lishi uchun
        var section = configuration.GetSection("Discovery");
        services.Configure<DiscoveryOptions>(section);

        // 📂 Cache (server IP’ni saqlab qolish uchun)
        var cachePath = section.GetValue<string>("CacheFilePath") ?? "discovery.cache";
        services.AddSingleton(new DiscoveryCache(cachePath));

        // 🌐 HttpClient factory (requires Microsoft.Extensions.Http)
        services.AddHttpClient("discovery");

        // 💡 Client discovery (faqat bitta interface orqali backend serverni topadi)
        services.AddSingleton<DiscoveryClient>();

        return services;
    }
}
