namespace VoltStream.Modules.Discovery.Client;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using VoltStream.Modules.Discovery.Abstractions;
using VoltStream.Modules.Discovery.Core;
using VoltStream.Modules.Discovery.Models;

public class DiscoveryClient(
    IServerDiscovery serverDiscovery,
    IOptions<DiscoveryOptions> opts,
    DiscoveryCache cache,
    ILogger<DiscoveryClient> logger,
    IHttpClientFactory httpClientFactory)
{
    private readonly DiscoveryOptions _options = opts.Value;

    public async Task<IPEndPoint?> GetServerAsync(CancellationToken cancellationToken = default)
    {
        // 1️⃣ Cache orqali
        var cached = cache.Load();
        if (cached != null)
        {
            logger.LogInformation("Using cached endpoint: {endpoint}", cached);
            return cached;
        }

        // 2️⃣ Discovery orqali topish
        var endpoint = await serverDiscovery.DiscoverAsync(cancellationToken);
        if (endpoint != null)
        {
            cache.Save(endpoint);
            return endpoint;
        }

        // 3️⃣ Agar enableRegistry bo‘lsa, markaziy registry orqali olish
        if (_options.EnableRegistry && !string.IsNullOrEmpty(_options.RegistryUrl))
        {
            try
            {
                var http = httpClientFactory.CreateClient("discovery");
                var url = $"{_options.RegistryUrl.TrimEnd('/')}/api/discovery/nodes/{_options.ServiceId}";
                var nodes = await http.GetFromJsonAsync<List<ServiceRegistration>>(url, cancellationToken);

                var node = nodes?.FirstOrDefault();
                if (node is not null)
                {
                    var ep = new IPEndPoint(IPAddress.Parse(node.Host), node.Port);
                    logger.LogInformation("Got endpoint from registry: {endpoint}", ep);
                    cache.Save(ep);
                    return ep;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to fetch from registry.");
            }
        }

        logger.LogWarning("No server found by any discovery method.");
        return null;
    }
}
