namespace VoltStream.WPF;

using ApiServices.Extensions;
using ApiServices.Interfaces;
using ApiServices.Services;
using Mapster;
using MapsterMapper;
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

        host = Host.CreateDefaultBuilder()
            .ConfigureServices((context, services) =>
            {
                ConfigureUiServices(services);
                ConfigureCoreServices(services);
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

    private static void ConfigureCoreServices(IServiceCollection services)
    {
        services.AddSingleton<DiscoveryClient>();
        services.AddSingleton<AppInitializer>();
        services.AddHostedService<ConnectionMonitor>();
        ApiService.ConfigureServices(services);
    }
}

public class DiscoveryClient
{
    private const int DiscoveryPort = 5001;
    private const int TimeoutMs = 2000;
    private const int MaxAttempts = 3;
    private const int RetryDelayMs = 2000;

    public static async Task<Uri?> DiscoverAsync()
    {
        for (int attempt = 1; attempt <= MaxAttempts; attempt++)
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

            await Task.Delay(RetryDelayMs);
        }

        return null;
    }
}

public class AppInitializer(IHostEnvironment env)
{
    public async Task InitializeAsync()
    {
        var uri = await DiscoveryClient.DiscoverAsync();
        if (uri is null)
            return;

        var host = env.IsDevelopment() ? "localhost" : uri.Host;
        var apiUrl = $"{uri.Scheme}://{host}:{uri.Port}/api";

        Console.WriteLine($"✅ Final API: {apiUrl}");

        var urlHolder = App.Services!.GetRequiredService<ApiUrlHolder>();
        urlHolder.Url = apiUrl;
    }
}

public class ConnectionMonitor(
    ApiUrlHolder urlHolder,
    IHostEnvironment env,
    IHealthCheckApi client)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var response = await client.GetAsync(stoppingToken).Handle();
            if (!response.IsSuccess)
                await TryRediscoverAsync();
            await Task.Delay(5000, stoppingToken);
        }
    }

    private async Task TryRediscoverAsync()
    {
        while (true)
        {
            var uri = await DiscoveryClient.DiscoverAsync();
            if (uri is not null)
            {
                var host = env.IsDevelopment() ? "localhost" : uri.Host;
                var newUrl = $"{uri.Scheme}://{host}:{uri.Port}/api";

                if (urlHolder.Url != newUrl)
                    urlHolder.Url = newUrl;

                return; // ✅ Rediscovery successful, exit loop
            }

            await Task.Delay(5000);
        }
    }
}
