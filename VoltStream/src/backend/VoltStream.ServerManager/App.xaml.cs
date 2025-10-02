namespace VoltStream.ServerManager;

using Microsoft.Extensions.Configuration;
using System.IO;
using System.Windows;
using VoltStream.ServerManager.Api;

public partial class App : Application
{
    public static IAllowedClientsApi AllowedClientsApi { get; private set; } = default!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("settings.json", optional: true, reloadOnChange: true)
            .Build();

        var port = config.GetValue("ServerPort", 5000);
        var host = config.GetValue("DatabaseHost", "localhost");

        var baseUrl = $"http://{host}:{port}/api";

        AllowedClientsApi = ApiFactory.CreateAllowedClients(baseUrl);
    }
}
