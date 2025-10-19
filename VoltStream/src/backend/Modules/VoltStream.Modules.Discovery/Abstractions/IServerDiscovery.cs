namespace VoltStream.Modules.Discovery.Abstractions;

using System.Net;

public interface IServerDiscovery
{
    /// <summary>
    /// Tries to find a server endpoint using discovery strategies.
    /// </summary>
    /// <returns>Discovered server endpoint or null if not found.</returns>
    Task<IPEndPoint?> DiscoverAsync(CancellationToken cancellationToken = default);
}
