namespace Discovery.Core;

using System.Net;
using System.Text.Json;

public class DiscoveryCache(string path)
{
    public void Save(IPEndPoint endpoint)
        => File.WriteAllText(path, $"{endpoint.Address}:{endpoint.Port}");

    public IPEndPoint? Load()
    {
        if (!File.Exists(path)) return null;
        var parts = File.ReadAllText(path).Split(':');
        if (parts.Length != 2) return null;
        return new IPEndPoint(IPAddress.Parse(parts[0]), int.Parse(parts[1]));
    }

    public void Save<T>(T data)
    {
        var json = JsonSerializer.Serialize(data);
        File.WriteAllText(path, json);
    }

    public T? Load<T>()
    {
        if (!File.Exists(path)) return default;
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<T>(json);
    }

    public void Clear()
    {
        if (File.Exists(path)) File.Delete(path);
    }
}
