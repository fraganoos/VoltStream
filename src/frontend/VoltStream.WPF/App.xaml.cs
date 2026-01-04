namespace VoltStream.WPF;

using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using VoltStream.WPF.Commons;
using VoltStream.WPF.Commons.Services;
using VoltStream.WPF.Configurations;
using VoltStream.WPF.LoginPages.Models;
using VoltStream.WPF.LoginPages.Views;

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
            }).Build();

        //await host.StartAsync();
        Services = host.Services;

        var loginWindow = Services.GetRequiredService<LoginWindow>();
        var loginViewModel = (LoginViewModel)loginWindow.DataContext!;

        loginViewModel.LoginSucceeded += () =>
        {
            var mainWindow = Services.GetRequiredService<MainWindow>();
            mainWindow.Show();
            loginWindow.Close();
        };

        loginWindow.Show();
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
        services.AddHostedService<ConnectionMonitor>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<ISessionService, SessionService>();
        services.AddSingleton<NamozTimeService>();
        ApiService.ConfigureServices(services);
    }
}
