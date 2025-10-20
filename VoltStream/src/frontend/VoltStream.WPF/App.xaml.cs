namespace VoltStream.WPF;

using ApiServices.Services;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using VoltStream.WPF.Commons;
using VoltStream.WPF.Configurations;

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

        await host.StartAsync();

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
