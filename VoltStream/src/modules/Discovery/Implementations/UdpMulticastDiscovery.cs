namespace Discovery.Implementations;

using Discovery.Abstractions;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class UdpMulticastDiscovery : IServerDiscovery
{
    private const int MulticastPort = 9999; // bir xil port client va server uchun
    private const string MulticastAddress = "239.0.0.222"; // multicast group IP (239.0.0.0–239.255.255.255)
    private const string DiscoverMessage = "DISCOVER_SERVER";
    private const int TimeoutMs = 3000; // 3 sekund kutish

    public async Task<IPEndPoint?> DiscoverAsync(CancellationToken cancellationToken = default)
    {
        using var udpClient = new UdpClient();
        udpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

        var multicastEp = new IPEndPoint(IPAddress.Parse(MulticastAddress), MulticastPort);

        // Multicast join
        udpClient.JoinMulticastGroup(multicastEp.Address);

        try
        {
            // So‘rov yuborish
            var requestBytes = Encoding.UTF8.GetBytes(DiscoverMessage);
            await udpClient.SendAsync(requestBytes, requestBytes.Length, multicastEp);

            // Receive javob (timeout bilan)
            var receiveTask = udpClient.ReceiveAsync();

            using var timeoutCts = new CancellationTokenSource(TimeoutMs);
            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

            var completed = await Task.WhenAny(receiveTask, Task.Delay(Timeout.Infinite, linkedCts.Token));

            if (completed == receiveTask)
            {
                var result = receiveTask.Result;
                var message = Encoding.UTF8.GetString(result.Buffer);

                if (message.StartsWith("SERVER_INFO"))
                {
                    // Masalan: SERVER_INFO:192.168.1.10:5000
                    var parts = message.Split(':', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 3 && int.TryParse(parts[2], out int port))
                    {
                        return new IPEndPoint(IPAddress.Parse(parts[1]), port);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[UdpMulticastDiscovery] Xatolik: {ex.Message}");
        }
        finally
        {
            udpClient.DropMulticastGroup(IPAddress.Parse(MulticastAddress));
        }

        return null;
    }
}
