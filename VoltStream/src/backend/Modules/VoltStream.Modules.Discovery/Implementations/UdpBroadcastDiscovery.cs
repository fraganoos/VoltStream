namespace VoltStream.Modules.Discovery.Implementations;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Sockets;
using System.Text;
using VoltStream.Modules.Discovery.Abstractions;
using VoltStream.Modules.Discovery.Core;
using VoltStream.Modules.Discovery.Models;

public class UdpBroadcastDiscovery(
    IOptions<DiscoveryOptions> opts, 
    ILogger<UdpBroadcastDiscovery> logger) : 
    IServerDiscovery
{
    private readonly DiscoveryOptions _options = opts.Value;

    public async Task<IPEndPoint?> DiscoverAsync(CancellationToken cancellationToken = default)
    {
        using var client = new UdpClient();
        client.EnableBroadcast = true;

        var nonce = Guid.NewGuid().ToString("N");
        var msg = $"DISCOVER:{_options.ServiceId}:{nonce}";
        var bytes = Encoding.UTF8.GetBytes(msg);
        var broadcastEp = new IPEndPoint(IPAddress.Broadcast, _options.DiscoveryPort);

        logger.LogInformation("Broadcast discovery started...");

        for (int i = 0; i < _options.BroadcastRetries; i++)
        {
            await client.SendAsync(bytes, bytes.Length, broadcastEp);

            var task = client.ReceiveAsync();
            var completed = await Task.WhenAny(task, Task.Delay(_options.BroadcastTimeoutMs, cancellationToken));
            if (completed == task)
            {
                var data = task.Result.Buffer;
                var response = Encoding.UTF8.GetString(data);
                if (response.StartsWith("SERVER:"))
                {
                    var parts = response.Split(':');
                    if (parts.Length >= 5)
                    {
                        var ip = parts[2];
                        var port = int.Parse(parts[3]);
                        var signature = parts[4];

                        var payload = $"SERVER:{_options.ServiceId}:{ip}:{port}:{nonce}";
                        if (SecurityHelper.VerifyHmac(payload, signature, _options.SharedSecret))
                        {
                            var endpoint = new IPEndPoint(IPAddress.Parse(ip), port);
                            logger.LogInformation("Server discovered: {endpoint}", endpoint);
                            return endpoint;
                        }
                    }
                }
            }
        }

        logger.LogWarning("Broadcast discovery failed.");
        return null;
    }
}
