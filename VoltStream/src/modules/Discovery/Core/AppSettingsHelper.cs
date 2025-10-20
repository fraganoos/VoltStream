namespace Discovery.Core;

using System.Text.Json;

public static class AppSettingsHelper
{
    private static string ConfigFile = "appsettings.json";

    public static void SetConfigPath(string path)
    {
        ConfigFile = path;
    }

    public static T? ReadSection<T>(string sectionName)
    {
        if (!File.Exists(ConfigFile)) return default;

        string json = File.ReadAllText(ConfigFile);
        using var doc = JsonDocument.Parse(json);

        if (!doc.RootElement.TryGetProperty(sectionName, out var section)) return default;

        return JsonSerializer.Deserialize<T>(section.GetRawText());
    }

    public static void UpdateServerConfig(string sectionName, string host, int port)
    {
        if (!File.Exists(ConfigFile)) return;

        string json = File.ReadAllText(ConfigFile);
        var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json)!;

        Dictionary<string, object> section;
        if (dict.TryGetValue(sectionName, out var rawSection))
        {
            string sectionJson = JsonSerializer.Serialize(rawSection);
            section = JsonSerializer.Deserialize<Dictionary<string, object>>(sectionJson)!;
        }
        else
        {
            section = [];
        }

        section["Host"] = host;
        section["Port"] = port;
        dict[sectionName] = section;

        string updated = JsonSerializer.Serialize(dict, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(ConfigFile, updated);
    }

    public static void UpdateApiBaseUrl(string url)
    {
        if (!File.Exists(ConfigFile)) return;

        string json = File.ReadAllText(ConfigFile);
        var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(json)!;

        dict["ApiBaseUrl"] = url;

        string updated = JsonSerializer.Serialize(dict, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(ConfigFile, updated);
    }
}
