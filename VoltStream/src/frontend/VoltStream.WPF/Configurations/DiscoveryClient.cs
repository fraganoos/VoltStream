namespace VoltStream.WPF.Configurations;

using System.Net;
using System.Net.Sockets;
using System.Text;

public class DiscoveryClient
{
    private const int DiscoveryPort = 5001;
    private const int TimeoutMs = 2000;
    private const int MaxAttempts = 3;
    private const int RetryDelayMs = 2000;

    public static async Task<Uri?> DiscoverAsync()
    {
        for (int attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            using var udp = new UdpClient();
            udp.EnableBroadcast = true;

            var request = Encoding.UTF8.GetBytes("DISCOVER");
            var broadcast = new IPEndPoint(IPAddress.Broadcast, DiscoveryPort);

            await udp.SendAsync(request, request.Length, broadcast);

            var receiveTask = udp.ReceiveAsync();
            var timeoutTask = Task.Delay(TimeoutMs);
            var completed = await Task.WhenAny(receiveTask, timeoutTask);

            if (completed == receiveTask)
            {
                var result = receiveTask.Result;
                var response = Encoding.UTF8.GetString(result.Buffer).Trim();

                if (Uri.TryCreate(response, UriKind.Absolute, out var uri))
                    return uri;
            }

            await Task.Delay(RetryDelayMs);
        }

        return null;
    }
}
