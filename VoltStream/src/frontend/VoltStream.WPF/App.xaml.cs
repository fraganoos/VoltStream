namespace VoltStream.WPF;

using ApiServices.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Forms.Design;

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
        ApiService.ConfigureServices(services, "https://localhost:7287");

        // WPF xizmatlarini qo‘shish
        services.AddSingleton<MainViewModel>();

        // Extension method ishlaydi
        var serviceProvider = services.BuildServiceProvider();

        var mainWindow = new MainWindow
        {
            DataContext = serviceProvider.GetService<MainViewModel>()
        };
        mainWindow.Show();
    }
}