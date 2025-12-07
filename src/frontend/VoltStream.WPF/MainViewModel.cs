using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using VoltStream.WPF.Commons;
using VoltStream.WPF.Commons.Services;
using VoltStream.WPF.Commons.ViewModels;
using VoltStream.WPF.Debitors.Views;
using VoltStream.WPF.Payments.Views;
using VoltStream.WPF.Products.Views;
using VoltStream.WPF.Sales.Views;
using VoltStream.WPF.Sales_history.Views;
using VoltStream.WPF.Settings.Views;
using VoltStream.WPF.Supplies.Views;
using VoltStream.WPF.Turnovers.Views;

public partial class MainViewModel : ViewModelBase
{
    private readonly INavigationService _navigationService;

    private readonly IServiceProvider _services;

    [ObservableProperty] private ApiConnectionViewModel apiConnection;
    [ObservableProperty] private object? currentChildView; // Made nullable
    [ObservableProperty] private string currentPageTitle = "Bosh sahifa";
    [ObservableProperty] private bool isSidebarCollapsed = false;

    public MainViewModel(IServiceProvider services, INavigationService navigationService)
    {
        _services = services;
        _navigationService = navigationService;

        ApiConnection = services.GetRequiredService<ApiConnectionViewModel>();

        // Subscribe to navigation changes
        if (_navigationService is NavigationService navImpl)
        {
            navImpl.PropertyChanged += NavigationService_PropertyChanged;
        }

        // Set initial view
        _navigationService.Navigate(_services.GetRequiredService<SalesPage>());
    }

    private void NavigationService_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(NavigationService.CurrentView))
        {
            CurrentChildView = _navigationService.CurrentView;
        }
    }



    [RelayCommand]
    private void ShowSalesView()
    {
        _navigationService.Navigate(_services.GetRequiredService<SalesPage>());
        CurrentPageTitle = "Savdo";
    }

    [RelayCommand]
    private void ShowSuppliesView()
    {
        _navigationService.Navigate(_services.GetRequiredService<SuppliesPage>());
        CurrentPageTitle = "Ishlab chiqarish";
    }

    [RelayCommand]
    private void ShowPaymentView()
    {
        _navigationService.Navigate(_services.GetRequiredService<PaymentsPage>());
        CurrentPageTitle = "Oldi-berdi";
    }

    [RelayCommand]
    private void ShowProductView()
    {
        _navigationService.Navigate(_services.GetRequiredService<ProductsPage>());
        CurrentPageTitle = "Mahsulotlar qoldig'i";
    }

    [RelayCommand]
    private void ShowSalesHistoryView()
    {
        _navigationService.Navigate(_services.GetRequiredService<SalesHistoryPage>());
        CurrentPageTitle = "Savdo Tarixi";
    }

    [RelayCommand]
    private void ShowDebitorCreditor()
    {
        _navigationService.Navigate(_services.GetRequiredService<DebitorCreditorPage>());
        CurrentPageTitle = "Debitor va Kreditor";
    }

    [RelayCommand]
    private void ShowTurnoversPage()
    {
        _navigationService.Navigate(_services.GetRequiredService<TurnoversPage>());
        CurrentPageTitle = "Oborotka";
    }

    [RelayCommand]
    private void ShowSettings()
    {
        _navigationService.Navigate(_services.GetRequiredService<SettingsPage>());
        CurrentPageTitle = "Sozlamalar";
    }

}
