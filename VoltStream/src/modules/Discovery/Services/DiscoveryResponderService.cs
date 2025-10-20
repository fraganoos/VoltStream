namespace Discovery.Services;

using Discovery.Core;
using Discovery.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class DiscoveryResponderService(
    IOptions<DiscoveryOptions> opts,
    ILogger<DiscoveryResponderService> logger) : BackgroundService
{
    private readonly DiscoveryOptions _options = opts.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var udpServer = new UdpClient(_options.DiscoveryPort);
        logger.LogInformation("📡 Discovery responder listening on UDP port {port}", _options.DiscoveryPort);

        while (!stoppingToken.IsCancellationRequested)
        {
            UdpReceiveResult result;
            try
            {
                result = await udpServer.ReceiveAsync();
            }
            catch (Exception ex)
            {
                logger.LogWarning("⚠️ UDP receive failed: {msg}", ex.Message);
                continue;
            }

            var msg = Encoding.UTF8.GetString(result.Buffer);
            if (!msg.StartsWith($"DISCOVER:{_options.ServiceId}:")) continue;

            var nonce = msg.Split(':').Last();

            // IP manzilni soketdan olish
            var ip = ((IPEndPoint)udpServer.Client.LocalEndPoint!).Address.ToString();

            var payload = $"SERVER:{_options.ServiceId}:{ip}:{_options.ServerPort}:{nonce}";
            var signature = SecurityHelper.ComputeHmac(payload, _options.SharedSecret);
            var response = Encoding.UTF8.GetBytes($"{payload}:{signature}");

            try
            {
                await udpServer.SendAsync(response, response.Length, result.RemoteEndPoint);
                logger.LogInformation("✅ Discovery response sent to {remote}", result.RemoteEndPoint);
            }
            catch (Exception ex)
            {
                logger.LogWarning("❌ Failed to send response: {msg}", ex.Message);
            }
        }
    }
}
