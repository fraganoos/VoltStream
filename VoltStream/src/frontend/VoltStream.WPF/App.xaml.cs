namespace VoltStream.WPF;

using ApiServices.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using VoltStream.WPF.Customer;
using VoltStream.WPF.LoginPages.Models;
using VoltStream.WPF.LoginPages.Views;
using VoltStream.WPF.Sales.Views;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var services = new ServiceCollection();

        // ApiService’dan API sozlamalarini olish
        ApiService.ConfigureServices(services, "https://localhost:7287/api");

        // WPF xizmatlarini qo‘shish
        services.AddSingleton<LoginViewModel>();
        services.AddSingleton<LoginWindow>();

        services.AddSingleton<MainWindow>();
        services.AddSingleton<MainViewModel>();
        services.AddTransient<SalesPage>();
        services.AddTransient<CustomerWindow>();

        // Extension method ishlaydi
        var serviceProvider = services.BuildServiceProvider();

        var loginViewModel = serviceProvider.GetRequiredService<LoginViewModel>();
        var loginWindow = serviceProvider.GetRequiredService<LoginWindow>();
        loginWindow.DataContext = loginViewModel;

        // ✅ Login muvaffaqiyatli bo‘lsa, LoginWindow yopilib MainWindow ochiladi
        loginViewModel.LoginSucceeded += () =>
        {
            loginWindow.Dispatcher.Invoke(() =>
            {

                var mainWindow = serviceProvider.GetRequiredService<MainWindow>();
                mainWindow.DataContext = serviceProvider.GetRequiredService<MainViewModel>();
                mainWindow.Show();
                loginWindow.Close();
            });
        };

        loginWindow.Show();


        //var mainWindow = new MainWindow
        //{
        //    DataContext = serviceProvider.GetService<MainViewModel>()
        //};
        //mainWindow.Show();
    }
}