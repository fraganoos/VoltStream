namespace VoltStream.Modules.Discovery.Implementations;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Sockets;
using VoltStream.Modules.Discovery.Abstractions;
using VoltStream.Modules.Discovery.Models;

public class IpScannerDiscovery(
    ILogger<IpScannerDiscovery> logger,
    IOptions<DiscoveryOptions> options) : 
    IServerDiscovery
{
    private readonly DiscoveryOptions _options = options.Value;

    public async Task<IPEndPoint?> DiscoverAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting IP scan in range {Range}", _options.IpRange);

        var (start, end) = ParseRange(_options.IpRange);

        var tasks = new List<Task<IPEndPoint?>>();

        for (uint ip = start; ip <= end; ip++)
        {
            var ipAddress = new IPAddress(BitConverter.GetBytes(ip).Reverse().ToArray());
            tasks.Add(CheckHostAsync(ipAddress, _options.Port, cancellationToken));
        }

        var results = await Task.WhenAll(tasks);
        return results.FirstOrDefault(r => r is not null);
    }

    private async Task<IPEndPoint?> CheckHostAsync(IPAddress ip, int port, CancellationToken ct)
    {
        try
        {
            using var client = new TcpClient();
            var connectTask = client.ConnectAsync(ip, port);

            var completed = await Task.WhenAny(connectTask, Task.Delay(300, ct));
            if (completed == connectTask && client.Connected)
            {
                logger.LogInformation("Discovered server at {Ip}:{Port}", ip, port);
                return new IPEndPoint(ip, port);
            }
        }
        catch
        {
            // ignore unreachable hosts
        }

        return null;
    }

    private static (uint start, uint end) ParseRange(string range)
    {
        var parts = range.Split('-');
        var startBytes = IPAddress.Parse(parts[0]).GetAddressBytes().Reverse().ToArray();
        var endBytes = IPAddress.Parse(parts[1]).GetAddressBytes().Reverse().ToArray();

        return (BitConverter.ToUInt32(startBytes, 0), BitConverter.ToUInt32(endBytes, 0));
    }
}
