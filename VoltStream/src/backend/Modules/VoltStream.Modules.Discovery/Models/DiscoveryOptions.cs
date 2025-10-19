namespace VoltStream.Modules.Discovery.Models;

public class DiscoveryOptions
{
    public string ServiceId { get; set; } = "VoltStream";
    public int DiscoveryPort { get; set; } = 5001;
    public int ServerPort { get; set; } = 5000;
    public string SharedSecret { get; set; } = "default_secret";

    public bool EnableMulticast { get; set; } = false;
    public string MulticastGroup { get; set; } = "239.0.0.222";

    public int BroadcastRetries { get; set; } = 3;
    public int BroadcastTimeoutMs { get; set; } = 2000;

    public bool EnableRegistry { get; set; } = false;
    public string? RegistryUrl { get; set; }
    public string? RegistryApiKey { get; set; }

    public string CacheFilePath { get; set; } = "discovery.cache";
    public bool IpScanEnabled { get; set; } = false;

    public string IpRange { get; set; } = "192.168.1.1-192.168.1.255";
    public int Port { get; set; } = 5000;
}
