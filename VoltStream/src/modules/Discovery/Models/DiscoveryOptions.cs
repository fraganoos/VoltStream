namespace Discovery.Models;

public class DiscoveryOptions
{
    // Xizmat identifikatori (UDP va registry uchun)
    public string ServiceId { get; set; } = "VoltStream";

    // Serverning HTTP host va porti (registry yoki health check uchun)
    public string Host { get; set; } = "localhost";
    public int ServerPort { get; set; } = 5000;

    // UDP discovery uchun port va secret
    public int DiscoveryPort { get; set; } = 5001;
    public string SharedSecret { get; set; } = "default_secret";

    // UDP broadcast sozlamalari
    public int BroadcastRetries { get; set; } = 3;
    public int BroadcastTimeoutMs { get; set; } = 2000;

    // UDP multicast sozlamalari
    public bool EnableMulticast { get; set; } = false;
    public string MulticastGroup { get; set; } = "239.0.0.222";

    // Registry API (agar ishlatilsa)
    public bool EnableRegistry { get; set; } = false;
    public string? RegistryUrl { get; set; }
    public string? RegistryApiKey { get; set; }

    // Discovery cache fayli
    public string CacheFilePath { get; set; } = "discovery.cache";

    // IP scanning (agar yoqilgan bo‘lsa)
    public bool IpScanEnabled { get; set; } = false;
    public string IpRange { get; set; } = "192.168.1.1-192.168.1.255";
}
