namespace VoltStream.WebApi.Utils;

using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class SimpleDiscoveryResponder(IServer server, IHostEnvironment env, IConfiguration config, ILogger<SimpleDiscoveryResponder> logger) : BackgroundService
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
                        var ip = ResolveIp();
                        var port = ResolvePort();
                        var scheme = ResolveScheme();
                        var response = $"{scheme}://{ip}:{port}";

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

    private string ResolveScheme()
    {
        var urls = config["ASPNETCORE_URLS"];
        if (urls?.Contains("https://") == true) return "https";
        if (urls?.Contains("http://") == true) return "http";

        var raw = _serverAddresses?.Addresses.FirstOrDefault();
        return Uri.TryCreate(raw, UriKind.Absolute, out var uri) ? uri.Scheme : "http";
    }

    private string ResolvePort()
    {
        // Docker-specific override
        var dockerHttp = config["ASPNETCORE_HTTP_PORTS"];
        var dockerHttps = config["ASPNETCORE_HTTPS_PORTS"];
        var dockerPort = config["DISCOVERY_PORT"];

        if (!string.IsNullOrWhiteSpace(dockerPort))
            return dockerPort;
        if (!string.IsNullOrWhiteSpace(dockerHttps))
            return dockerHttps;
        if (!string.IsNullOrWhiteSpace(dockerHttp))
            return dockerHttp;

        // Fallback to ASPNETCORE_URLS
        var urls = config["ASPNETCORE_URLS"];
        var uri = urls?.Split(';').FirstOrDefault();
        if (Uri.TryCreate(uri, UriKind.Absolute, out var parsed))
            return parsed.Port.ToString();

        // Fallback to IServerAddressesFeature
        var raw = _serverAddresses?.Addresses.FirstOrDefault();
        if (Uri.TryCreate(raw, UriKind.Absolute, out var fallback))
            return fallback.Port.ToString();

        return "7285"; // default
    }


    private string ResolveIp()
    {
        if (env.IsDevelopment())
            return "localhost";

        // Docker gateway IP (host IP from container)
        if (!string.IsNullOrWhiteSpace(config["DISCOVERY_PORT"]))
        {
            var dockerGateway = GetDockerGatewayIp();
            if (dockerGateway is not null)
                return dockerGateway;
        }

        // Productionda global IP
        var global = GetGlobalIp();
        return global ?? "localhost";
    }

    private string? GetDockerGatewayIp()
    {
        try
        {
            using var client = new UdpClient();
            client.Connect("8.8.8.8", 80); // Google DNS
            var endpoint = client.Client.LocalEndPoint as IPEndPoint;
            return endpoint?.Address.ToString();
        }
        catch
        {
            return null;
        }
    }

    private string? GetGlobalIp()
    {
        try
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            return host.AddressList
                .Where(ip => ip.AddressFamily == AddressFamily.InterNetwork)
                .Where(ip => !IPAddress.IsLoopback(ip))
                .Where(ip => !ip.ToString().StartsWith("169.254")) // ignore link-local
                .OrderByDescending(ip => ip.ToString().StartsWith("192.") || ip.ToString().StartsWith("10.") || ip.ToString().StartsWith("172."))
                .FirstOrDefault()
                ?.ToString();
        }
        catch
        {
            return null;
        }
    }
}
