namespace Discovery.Implementations;

using Discovery.Abstractions;
using Discovery.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Sockets;

public class IpScannerDiscovery(
    ILogger<IpScannerDiscovery> logger,
    IOptions<DiscoveryOptions> options) : IServerDiscovery
{
    private readonly DiscoveryOptions _options = options.Value;

    public async Task<IPEndPoint?> DiscoverAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("🔍 Starting IP scan in range {Range}", _options.IpRange);

        var (start, end) = ParseRange(_options.IpRange);
        var tasks = new List<Task<IPEndPoint?>>();

        for (uint ip = start; ip <= end; ip++)
        {
            var ipAddress = UInt32ToIp(ip);
            tasks.Add(CheckHostAsync(ipAddress, _options.ServerPort, cancellationToken));
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
                logger.LogInformation("✅ Discovered server at {Ip}:{Port}", ip, port);
                return new IPEndPoint(ip, port);
            }
        }
        catch
        {
            // unreachable host — ignore
        }

        return null;
    }

    private static (uint start, uint end) ParseRange(string range)
    {
        var parts = range.Split('-', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2)
            throw new ArgumentException("Invalid IP range format. Expected: 'start-end'");

        return (IpToUInt32(IPAddress.Parse(parts[0])), IpToUInt32(IPAddress.Parse(parts[1])));
    }

    private static uint IpToUInt32(IPAddress ip)
        => BitConverter.ToUInt32([.. ip.GetAddressBytes().Reverse()], 0);

    private static IPAddress UInt32ToIp(uint ip)
        => new([.. BitConverter.GetBytes(ip).Reverse()]);
}
