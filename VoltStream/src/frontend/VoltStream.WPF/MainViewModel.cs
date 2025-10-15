namespace VoltStream.WPF;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VoltStream.WPF.Commons;
using VoltStream.WPF.Payments.Views;
using VoltStream.WPF.Products.Views;
using VoltStream.WPF.Sales.Views;
using VoltStream.WPF.Sales_history.Views;
using VoltStream.WPF.Settings.Views;
using VoltStream.WPF.Supplies.Views;

public partial class MainViewModel : ViewModelBase
{
    private readonly IServiceProvider serviceProvider;

    [ObservableProperty] private object currentChildView;
    [ObservableProperty] private string currentPageTitle = "Bosh sahifa";

    public MainViewModel(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
        CurrentChildView = new SalesPage(serviceProvider);
    }

    [RelayCommand]
    private void ShowSalesView()
    {
        CurrentChildView = new SalesPage(serviceProvider);
        CurrentPageTitle = "Savdo";
    }

    [RelayCommand]
    private void ShowSuppliesView()
    {
        CurrentChildView = new SuppliesPage(serviceProvider);
        CurrentPageTitle = "Ishlab chiqarish";
    }

    [RelayCommand]
    private void ShowPaymentView()
    {
        CurrentChildView = new PaymentsPage(serviceProvider);
        CurrentPageTitle = "Oldi-berdi";
    }

    [RelayCommand]
    private void ShowProductView()
    => CurrentChildView = new ProductsPage(serviceProvider);

    [RelayCommand]
    private void ShowSalesHistoryView()
    => CurrentChildView = new SalesHistoryPage(serviceProvider);
    {
        CurrentChildView = new ProductsPage(serviceProvider);
        CurrentPageTitle = "Mahsulotlar qoldig'i";
    }

    [RelayCommand]
    private void ShowSettings()
    {
        CurrentChildView = new SettingsPage();
        CurrentPageTitle = "Sozlamalar";
    }
}
