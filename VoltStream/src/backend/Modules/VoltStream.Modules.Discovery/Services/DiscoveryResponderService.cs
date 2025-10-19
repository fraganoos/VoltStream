namespace VoltStream.Modules.Discovery.Services;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Sockets;
using System.Text;
using VoltStream.Modules.Discovery.Core;
using VoltStream.Modules.Discovery.Models;

public class DiscoveryResponderService(
    IOptions<DiscoveryOptions> opts, 
    ILogger<DiscoveryResponderService> logger) : 
    BackgroundService
{
    private readonly DiscoveryOptions _options = opts.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var udpServer = new UdpClient(_options.DiscoveryPort);
        logger.LogInformation("Discovery responder listening on UDP {port}", _options.DiscoveryPort);

        while (!stoppingToken.IsCancellationRequested)
        {
            var result = await udpServer.ReceiveAsync(stoppingToken);
            var msg = Encoding.UTF8.GetString(result.Buffer);

            if (msg.StartsWith($"DISCOVER:{_options.ServiceId}:"))
            {
                var nonce = msg.Split(':').Last();
                var ip = GetLocalIPAddress();
                var payload = $"SERVER:{_options.ServiceId}:{ip}:{_options.ServerPort}:{nonce}";
                var signature = SecurityHelper.ComputeHmac(payload, _options.SharedSecret);
                var response = Encoding.UTF8.GetBytes($"{payload}:{signature}");
                await udpServer.SendAsync(response, response.Length, result.RemoteEndPoint);
            }
        }
    }

    private static string GetLocalIPAddress()
    {
        return Dns.GetHostAddresses(Dns.GetHostName())
            .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork)?
            .ToString() ?? "127.0.0.1";
    }
}
