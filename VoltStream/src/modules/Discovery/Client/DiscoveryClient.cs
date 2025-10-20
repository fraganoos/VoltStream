namespace Discovery.Client;

using Discovery.Core;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class DiscoveryClient(
    ILogger<DiscoveryClient> logger,
    DiscoveryCache cache,
    IHttpClientFactory httpClientFactory)
{
    public async Task<IPEndPoint?> DiscoverServerAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("🔍 Starting discovery process...");

        // 1. Avval cache’dan o‘qib ko‘r
        var cached = cache.Load();
        if (cached is not null)
        {
            logger.LogInformation("✅ Using cached endpoint: {0}", cached);
            if (await CheckServerAvailableAsync(cached))
                return cached;
        }

        // 2. UDP yoki IP scanning orqali qidirish (soddalashgan)
        var potential = await ScanForServerAsync(cancellationToken);
        if (potential is not null)
        {
            cache.Save(potential);
            logger.LogInformation("✅ Server found: {0}", potential);
            return potential;
        }

        logger.LogWarning("❌ No server found during discovery.");
        return null;
    }

    private async static Task<IPEndPoint?> ScanForServerAsync(CancellationToken cancellationToken)
    {
        const int DiscoveryPort = 5001; // bu server tarafida DiscoveryOptions.DiscoveryPort bilan mos bo‘lishi kerak
        const string ServiceId = "VoltStream"; // yoki configdan o‘qish ham mumkin
        const string SharedSecret = "SuperSecretKey123!"; // shu ham configdan kelishi kerak

        using var udpClient = new UdpClient();
        udpClient.EnableBroadcast = true;

        var nonce = Guid.NewGuid().ToString("N");
        var request = $"DISCOVER:{ServiceId}:{nonce}";
        var requestBytes = Encoding.UTF8.GetBytes(request);
        var broadcastEp = new IPEndPoint(IPAddress.Broadcast, DiscoveryPort);

        try
        {
            // 3 marta broadcast yuborib ko‘ramiz
            for (int attempt = 1; attempt <= 3; attempt++)
            {
                await udpClient.SendAsync(requestBytes, requestBytes.Length, broadcastEp);

                // 2 soniya ichida javob kutamiz
                var receiveTask = udpClient.ReceiveAsync();
                var completed = await Task.WhenAny(receiveTask, Task.Delay(2000, cancellationToken));

                if (completed == receiveTask)
                {
                    var result = receiveTask.Result;
                    var responseText = Encoding.UTF8.GetString(result.Buffer);

                    // Kutilgan format: SERVER:{ServiceId}:{ip}:{port}:{nonce}:{signature}
                    if (responseText.StartsWith("SERVER:"))
                    {
                        var parts = responseText.Split(':');
                        if (parts.Length >= 6)
                        {
                            var serviceId = parts[1];
                            var ip = parts[2];
                            var port = int.Parse(parts[3]);
                            var responseNonce = parts[4];
                            var signature = parts[5];

                            var payload = $"SERVER:{serviceId}:{ip}:{port}:{responseNonce}";
                            if (serviceId == ServiceId &&
                                responseNonce == nonce &&
                                SecurityHelper.VerifyHmac(payload, signature, SharedSecret))
                            {
                                return new IPEndPoint(IPAddress.Parse(ip), port);
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[DiscoveryClient] UDP scan xatolik: {ex.Message}");
        }

        return null;
    }

    private async Task<bool> CheckServerAvailableAsync(IPEndPoint endpoint)
    {
        try
        {
            var http = httpClientFactory.CreateClient("discovery");
            http.BaseAddress = new Uri($"http://{endpoint.Address}:{endpoint.Port}");
            var response = await http.GetAsync("/health");
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }
}