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
    /// <summary>
    /// Server tarafda discovery modulini ro‘yxatdan o‘tkazadi (UDP responder + strategiyalar)
    /// </summary>
    public static IServiceCollection AddDiscoveryModule(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection("Discovery");
        services.Configure<DiscoveryOptions>(section);

        var cacheFile = section.GetValue<string>("CacheFilePath") ?? "discovery.cache";
        services.AddSingleton(new DiscoveryCache(cacheFile));

        // Discovery strategiyalari
        services.AddSingleton<UdpBroadcastDiscovery>();
        services.AddSingleton<UdpMulticastDiscovery>();
        services.AddSingleton<IpScannerDiscovery>();

        // Discovery manager
        services.AddSingleton<IServerDiscovery>(sp =>
        {
            var strategies = new IServerDiscovery[]
            {
                sp.GetRequiredService<UdpBroadcastDiscovery>(),
                sp.GetRequiredService<UdpMulticastDiscovery>(),
                sp.GetRequiredService<IpScannerDiscovery>()
            };

            return new DiscoveryManager(
                strategies,
                sp.GetRequiredService<DiscoveryCache>(),
                sp.GetRequiredService<ILogger<DiscoveryManager>>()
            );
        });

        // ❗ HostedService faqat serverda ro‘yxatdan o‘tadi
        services.AddHostedService<DiscoveryResponderService>();

        return services;
    }

    /// <summary>
    /// Client tarafda discovery modulini ro‘yxatdan o‘tkazadi (UDP scanner + cache)
    /// </summary>
    public static IServiceCollection AddDiscoveryClient(this IServiceCollection services, IConfiguration configuration)
    {
        var section = configuration.GetSection("Discovery");
        services.Configure<DiscoveryOptions>(section);

        var cachePath = section.GetValue<string>("CacheFilePath") ?? "discovery.cache";
        services.AddSingleton(new DiscoveryCache(cachePath));

        services.AddHttpClient(); // health check uchun

        services.AddSingleton<DiscoveryClient>(); // UDP scanner + registry client

        return services;
    }
}
