namespace VoltStream.WPF.Configurations;

using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using VoltStream.WPF.Commons.ViewModels;

public class ApiConnectionStore
{
    private const string ConfigPath = "config/api-connection.json";
    private readonly JsonSerializerOptions jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new JsonStringEnumConverter() }
    };

    public ApiConnectionViewModel Load()
    {
        if (!File.Exists(ConfigPath))
            return new ApiConnectionViewModel();

        var json = File.ReadAllText(ConfigPath);
        return JsonSerializer.Deserialize<ApiConnectionViewModel>(json, jsonOptions)
               ?? new() { AutoReconnectEnabled = true };
    }

    public void Save(ApiConnectionViewModel model)
    {
        var json = JsonSerializer.Serialize(model, jsonOptions);
        Directory.CreateDirectory(Path.GetDirectoryName(ConfigPath)!);
        File.WriteAllText(ConfigPath, json);
    }

    public void BindAutoSave(ApiConnectionViewModel model)
    {
        model.PropertyChanged += (_, e) =>
        {
            if (e.PropertyName is nameof(model.Url) or nameof(model.AutoReconnectEnabled))
                Save(model);
        };
    }
}
