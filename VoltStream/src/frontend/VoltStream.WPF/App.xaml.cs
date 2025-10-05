namespace VoltStream.WPF;

using ApiServices.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Windows;
using VoltStream.WPF.Customer;
using VoltStream.WPF.LoginPages.Models;
using VoltStream.WPF.LoginPages.Views;
using VoltStream.WPF.Sales.Views;

public partial class App : Application
{
    public IServiceProvider? Services { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();

        // ✅ API konfiguratsiyasi
        ApiService.ConfigureServices(services, "https://localhost:7287/api");

        // ✅ ViewModel va View'larni ro‘yxatga olish
        services.AddSingleton<LoginViewModel>();
        services.AddSingleton<LoginWindow>();
        services.AddSingleton<MainViewModel>();
        services.AddTransient<MainWindow>();
        services.AddTransient<SalesPage>();
        services.AddTransient<CustomerWindow>();

        // ✅ DI konteynerni yaratish
        Services = services.BuildServiceProvider();

        // ⚙️ Login qismini hozircha o‘tkazamiz (agar kerak bo‘lsa qayta qo‘shamiz)
        var mainWindow = Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }
}
