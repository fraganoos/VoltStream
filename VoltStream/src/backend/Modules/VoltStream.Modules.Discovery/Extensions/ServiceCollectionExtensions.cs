namespace VoltStream.Modules.Discovery.Extensions;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VoltStream.Modules.Discovery.Abstractions;
using VoltStream.Modules.Discovery.Client;
using VoltStream.Modules.Discovery.Core;
using VoltStream.Modules.Discovery.Implementations;
using VoltStream.Modules.Discovery.Models;
using VoltStream.Modules.Discovery.Services;

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
    public static IServiceCollection AddDiscoveryClient(this IServiceCollection services)
    {
        services.AddHttpClient("discovery");
        services.AddSingleton<DiscoveryClient>();
        return services;
    }
}
