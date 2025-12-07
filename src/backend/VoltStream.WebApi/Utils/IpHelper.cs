namespace VoltStream.WebApi.Utils;

using System.Net;

public static class IpHelper
{
    public static bool IsLocal(IPAddress? ipAddress)
    {
        if (ipAddress is null) return false;
        if (IPAddress.IsLoopback(ipAddress)) return true;

        var localIps = Dns.GetHostAddresses(Dns.GetHostName());
        return localIps.Contains(ipAddress);
    }
}
