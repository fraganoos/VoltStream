namespace Discovery.Core;

using System.Net;

public class DiscoveryCache(string path)
{
    public void Save(IPEndPoint endpoint)
    {
        File.WriteAllText(path, $"{endpoint.Address}:{endpoint.Port}");
    }

    public IPEndPoint? Load()
    {
        if (!File.Exists(path)) return null;
        var parts = File.ReadAllText(path).Split(':');
        if (parts.Length != 2) return null;
        return new IPEndPoint(IPAddress.Parse(parts[0]), int.Parse(parts[1]));
    }

    public void Clear()
    {
        if (File.Exists(path)) File.Delete(path);
    }
}
