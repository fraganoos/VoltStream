namespace VoltStream.WPF;

using ApiServices.Services;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using VoltStream.WPF.Commons;

public partial class App : Application
{
    public static IServiceProvider? Services { get; private set; }
    private IHost? host;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        const string configPath = "appsettings.json";
        host = Host.CreateDefaultBuilder().ConfigureAppConfiguration(config =>
        {
            config.AddJsonFile(configPath, optional: false, reloadOnChange: true);

        })
        .ConfigureServices((context, services) =>
        {
            ConfigureUiServices(services);
            ConfigureCoreServices(services, context.Configuration);
        })
        .ConfigureLogging(logging =>
        {
            logging.ClearProviders();
            logging.AddConsole();
            logging.AddDebug();
        }).Build();

        Services = host.Services;

        var initializer = Services.GetRequiredService<AppInitializer>();
        await initializer.InitializeAsync();

        var mainWindow = Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }
    protected override async void OnExit(ExitEventArgs e)
    {
        if (host is not null)
            await host.StopAsync();

        host?.Dispose();
        base.OnExit(e);
    }

    private static void ConfigureUiServices(IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();

        var assembly = Assembly.GetExecutingAssembly();

        void RegisterByBaseType<TBase>(ServiceLifetime lifetime)
        {
            var types = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && typeof(TBase).IsAssignableFrom(t));

            foreach (var type in types)
                services.Add(new ServiceDescriptor(type, type, lifetime));
        }

        RegisterByBaseType<Window>(ServiceLifetime.Singleton);
        RegisterByBaseType<Page>(ServiceLifetime.Transient);
        RegisterByBaseType<ViewModelBase>(ServiceLifetime.Transient);
    }

    private static void ConfigureCoreServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<DiscoveryClient>();
        services.AddSingleton<AppInitializer>();
        var baseApiUrl = configuration["ApiBaseUrl"] ?? "https://localhost:7287/api";
        ApiService.ConfigureServices(services, baseApiUrl);
    }
}

public class DiscoveryClient
{
    private const int DiscoveryPort = 5001;
    private const int TimeoutMs = 2000;

    public async Task<Uri?> DiscoverAsync()
    {
        using var udp = new UdpClient();
        udp.EnableBroadcast = true;

        var request = Encoding.UTF8.GetBytes("DISCOVER");
        var broadcast = new IPEndPoint(IPAddress.Broadcast, DiscoveryPort);

        await udp.SendAsync(request, request.Length, broadcast);

        var receiveTask = udp.ReceiveAsync();
        var timeoutTask = Task.Delay(TimeoutMs);
        var completed = await Task.WhenAny(receiveTask, timeoutTask);

        if (completed == receiveTask)
        {
            var result = receiveTask.Result;
            var response = Encoding.UTF8.GetString(result.Buffer).Trim();

            if (Uri.TryCreate(response, UriKind.Absolute, out var uri))
                return uri;
        }

        return null;
    }
}

public class AppInitializer(DiscoveryClient discoveryClient, IHostEnvironment env)
{
    public async Task InitializeAsync()
    {
        var uri = await discoveryClient.DiscoverAsync();
        if (uri is null)
        {
            Console.WriteLine("❌ Server not found via UDP.");
            return;
        }

        var host = env.IsDevelopment() ? "localhost" : uri.Host;
        var apiUrl = $"{uri.Scheme}://{host}:{uri.Port}/api";

        Console.WriteLine($"✅ Final API: {apiUrl}");
        ApiService.Reconfigure(App.Services!, apiUrl);
    }
}