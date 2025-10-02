namespace VoltStream.ServerManager.Models;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json.Serialization;

public partial class AppSettings : ObservableObject
{
    [ObservableProperty] private Server server = new();
    [ObservableProperty] private ConnectionStrings connectionStrings = new();
}

public partial class ConnectionStrings : ObservableObject
{
    public string DefaultConnection { get; set; } = "host=localhost;port=5432;database=voltstream;uid=postgres;password=root;";

    [ObservableProperty]
    [JsonIgnore]
    private string databaseHost = "localhost";

    [ObservableProperty]
    [JsonIgnore]
    private int databasePort = 5432;

    [ObservableProperty]
    [JsonIgnore]
    private string databaseName = "voltstream";

    [ObservableProperty]
    [JsonIgnore]
    private string username = "postgres";

    [ObservableProperty]
    [JsonIgnore]
    private string password = "root";

    partial void OnDatabaseHostChanged(string value) => UpdateDefaultConnection();
    partial void OnDatabasePortChanged(int value) => UpdateDefaultConnection();
    partial void OnDatabaseNameChanged(string value) => UpdateDefaultConnection();
    partial void OnUsernameChanged(string value) => UpdateDefaultConnection();
    partial void OnPasswordChanged(string value) => UpdateDefaultConnection();

    private void UpdateDefaultConnection()
    {
        DefaultConnection = $"host={DatabaseHost};port={DatabasePort};database={DatabaseName};uid={Username};password={Password};";
    }
}

public partial class Server : ObservableObject
{
    [ObservableProperty] private int port = 5000;
    [ObservableProperty] private bool useHttps = false;
}
