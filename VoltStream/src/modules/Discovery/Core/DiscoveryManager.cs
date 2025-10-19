namespace Discovery.Core;

using Discovery.Abstractions;
using Microsoft.Extensions.Logging;
using System.Net;

public class DiscoveryManager(
    IEnumerable<IServerDiscovery> strategies,
    DiscoveryCache cache,
    ILogger<DiscoveryManager> logger) :
    IServerDiscovery
{
    public async Task<IPEndPoint?> DiscoverAsync(CancellationToken cancellationToken = default)
    {
        var cached = cache.Load();
        if (cached is not null)
        {
            logger.LogInformation("Cache found: {cached}", cached);
            return cached;
        }

        foreach (var strategy in strategies)
        {
            var result = await strategy.DiscoverAsync(cancellationToken);
            if (result != null)
            {
                cache.Save(result);
                return result;
            }
        }

        logger.LogWarning("All discovery strategies failed.");
        return null;
    }
}
