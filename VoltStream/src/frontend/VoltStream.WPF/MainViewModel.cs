namespace VoltStream.WPF;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using VoltStream.WPF.Commons;
using VoltStream.WPF.Commons.ViewModels;
using VoltStream.WPF.Debitors.Views;
using VoltStream.WPF.Payments.Views;
using VoltStream.WPF.Products.Views;
using VoltStream.WPF.Sales.Views;
using VoltStream.WPF.Sales_history.Views;
using VoltStream.WPF.Settings.Views;
using VoltStream.WPF.Supplies.Views;
using VoltStream.WPF.Turnovers.Views;

public partial class MainViewModel(IServiceProvider services) : ViewModelBase
{
    [ObservableProperty] private ApiConnectionViewModel apiConnection = services.GetRequiredService<ApiConnectionViewModel>();
    [ObservableProperty] private object currentChildView = services.GetRequiredService<SalesPage>();
    [ObservableProperty] private string currentPageTitle = "Bosh sahifa";
    [ObservableProperty] private bool isSidebarCollapsed = false;


    [RelayCommand]
    private void ShowSalesView()
    {
        CurrentChildView = services.GetRequiredService<SalesPage>();
        CurrentPageTitle = "Savdo";
    }

    [RelayCommand]
    private void ShowSuppliesView()
    {
        CurrentChildView = services.GetRequiredService<SuppliesPage>();
        CurrentPageTitle = "Ishlab chiqarish";
    }

    [RelayCommand]
    private void ShowPaymentView()
    {
        CurrentChildView = services.GetRequiredService<PaymentsPage>();
        CurrentPageTitle = "Oldi-berdi";
    }

    [RelayCommand]
    private void ShowProductView()
    {
        CurrentChildView = services.GetRequiredService<ProductsPage>();
        CurrentPageTitle = "Mahsulotlar qoldig'i";
    }

    [RelayCommand]
    private void ShowSalesHistoryView()
    {
        CurrentChildView = services.GetRequiredService<SalesHistoryPage>();
        CurrentPageTitle = "Savdo Tarixi";
    }

    [RelayCommand]
    private void ShowDebitorCreditor()
    {
        CurrentChildView = services.GetRequiredService<DebitorCreditorPage>();
        CurrentPageTitle = "Debitor va Kreditor";
    }

    [RelayCommand]
    private void ShowTurnoversPage()
    {
        CurrentChildView = services.GetRequiredService<TurnoversPage>();
        CurrentPageTitle = "Oborotka";
    }

    [RelayCommand]
    private void ShowSettings()
    {
        CurrentChildView = services.GetRequiredService<SettingsPage>();
        CurrentPageTitle = "Sozlamalar";
    }

}
