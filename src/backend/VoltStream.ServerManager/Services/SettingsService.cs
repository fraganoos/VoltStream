namespace VoltStream.ServerManager.Services;

using System.IO;
using System.Text.Json;
using VoltStream.ServerManager.Models;

public static class SettingsService
{
    private static readonly string FilePath = "appsettings.json";
    private static readonly JsonSerializerOptions CachedJsonOptions = new() { WriteIndented = true };

    public static AppSettings Load()
    {
        if (!File.Exists(FilePath))
        {
            var defaultSettings = new AppSettings();
            Save(defaultSettings);
            return defaultSettings;
        }

        var json = File.ReadAllText(FilePath);
        return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
    }

    public static void Save(AppSettings settings)
    {
        var json = JsonSerializer.Serialize(settings, CachedJsonOptions);
        File.WriteAllText(FilePath, json);
    }
}
