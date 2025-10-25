namespace VoltStream.WebApi.Utils;

using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class SimpleDiscoveryResponder(IServer server, IHostEnvironment env, ILogger<SimpleDiscoveryResponder> logger) : BackgroundService
{
    private const int ListenPort = 5001;
    private readonly IServerAddressesFeature? _serverAddresses = server.Features.Get<IServerAddressesFeature>();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var udp = new UdpClient(ListenPort);
        logger.LogInformation("📡 Discovery responder listening on UDP port {port}", ListenPort);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (udp.Available > 0)
                {
                    var result = await udp.ReceiveAsync(stoppingToken);
                    var msg = Encoding.UTF8.GetString(result.Buffer).Trim();

                    if (msg == "DISCOVER")
                    {
                        var uri = GetServerUri();
                        var ip = GetLocalIp();
                        var response = $"{uri?.Scheme ?? "http"}://{ip}:{uri?.Port ?? 5000}";

                        var bytes = Encoding.UTF8.GetBytes(response);
                        await udp.SendAsync(bytes, bytes.Length, result.RemoteEndPoint);
                        logger.LogInformation("✅ Sent discovery response: {response} to {remote}", response, result.RemoteEndPoint);
                    }
                }

                await Task.Delay(10, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogWarning("⚠️ Discovery responder error: {msg}", ex.Message);
                await Task.Delay(100, stoppingToken);
            }
        }
    }

    private Uri? GetServerUri()
    {
        var raw = _serverAddresses?.Addresses.FirstOrDefault();
        return Uri.TryCreate(raw, UriKind.Absolute, out var uri) ? uri : null;
    }

    private string GetLocalIp()
    {
        if (env.IsDevelopment())
            return "localhost";

        var host = Dns.GetHostEntry(Dns.GetHostName());

        // Prioritize real network IPs (Ethernet, Wi-Fi)
        var preferred = host.AddressList
            .Where(ip => ip.AddressFamily == AddressFamily.InterNetwork)
            .Where(ip => !IPAddress.IsLoopback(ip))
            .Where(ip => ip.ToString().StartsWith("192."))
            .FirstOrDefault();

        return preferred?.ToString() ?? "localhost";
    }
}
