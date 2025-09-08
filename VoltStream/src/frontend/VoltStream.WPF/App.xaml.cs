namespace VoltStream.WPF;

using System.Windows;
using ApiServices.Services;
using System.Windows.Forms.Design;
using Microsoft.Extensions.DependencyInjection;

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
        services.AddSingleton<MineViewModel>();

        // Extension method ishlaydi
        var serviceProvider = services.BuildServiceProvider();

        var mainWindow = new MainWindow
        {
            DataContext = serviceProvider.GetService<MineViewModel>()
        };
        mainWindow.Show();
    }
}