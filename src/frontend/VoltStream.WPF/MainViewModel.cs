namespace VoltStream.WPF.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel;
using VoltStream.WPF.Commons;
using VoltStream.WPF.Commons.Services;
using VoltStream.WPF.Commons.ViewModels;
using VoltStream.WPF.Debitors.Views;
using VoltStream.WPF.Home.Views;
using VoltStream.WPF.Payments.Views;
using VoltStream.WPF.Products.Views;
using VoltStream.WPF.Sales.Views;
using VoltStream.WPF.Sales_history.Views;
using VoltStream.WPF.Settings.Views;
using VoltStream.WPF.Supplies.Views;
using VoltStream.WPF.Turnovers.Views;

public partial class MainViewModel : ViewModelBase
{
    private readonly INavigationService navigationService;
    private readonly IServiceProvider services;
    private readonly NamozTimeService namozService;

    [ObservableProperty] private ApiConnectionViewModel apiConnection;
    [ObservableProperty] private object? currentChildView;
    [ObservableProperty] private string currentPageTitle = "Bosh sahifa";
    [ObservableProperty] private bool isSidebarCollapsed = false;

    [ObservableProperty] private string bomdod = string.Empty;
    [ObservableProperty] private string quyosh = string.Empty;
    [ObservableProperty] private string peshin = string.Empty;
    [ObservableProperty] private string asr = string.Empty;
    [ObservableProperty] private string shom = string.Empty;
    [ObservableProperty] private string xufton = string.Empty;
    [ObservableProperty] private string dateLabel = string.Empty;
    [ObservableProperty] private string regionName = string.Empty;

    public MainViewModel(IServiceProvider services, INavigationService navigationService, NamozTimeService namozService)
    {
        this.services = services;
        this.navigationService = navigationService;
        this.namozService = namozService;

        ApiConnection = services.GetRequiredService<ApiConnectionViewModel>();

        if (this.navigationService is NavigationService navImpl)
        {
            navImpl.PropertyChanged += NavigationService_PropertyChanged;
        }

        this.navigationService.Navigate(this.services.GetRequiredService<DashboardPage>());
    }

    public async Task LoadNamozTimesAsync()
    {
        var data = await namozService.GetFullDataAsync();
        if (data == null || data.PeriodTable == null) return;

        string todayStr = DateTime.Now.ToString("dd.MM.yyyy");
        var todayData = data.PeriodTable.FirstOrDefault(x => x.Date == todayStr);
        var times = todayData?.Times ?? data.Today.Times;

        Bomdod = times.Bomdod;
        Quyosh = times.Quyosh;
        Peshin = times.Peshin;
        Asr = times.Asr;
        Shom = times.Shom;
        Xufton = times.Xufton;

        DateLabel = todayData?.Date ?? DateTime.Now.ToString("dd.MM.yyyy");
        RegionName = data.Meta.Region.Name;
    }

    private void NavigationService_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(NavigationService.CurrentView))
            CurrentChildView = navigationService.CurrentView;
    }

    [RelayCommand] private void ShowDashboardView() => NavigateTo<DashboardPage>("Bosh sahifa");
    [RelayCommand] private void ShowSalesView() => NavigateTo<SalesPage>("Savdo");
    [RelayCommand] private void ShowSuppliesView() => NavigateTo<SuppliesPage>("Ishlab chiqarish");
    [RelayCommand] private void ShowPaymentView() => NavigateTo<PaymentsPage>("To'lov");
    [RelayCommand] private void ShowProductView() => NavigateTo<ProductsPage>("Mahsulotlar qoldig'i");
    [RelayCommand] private void ShowSalesHistoryView() => NavigateTo<SalesHistoryPage>("Sotuv tahlili");
    [RelayCommand] private void ShowDebitorCreditor() => NavigateTo<DebitorCreditorPage>("Kontragentlar");
    [RelayCommand] private void ShowTurnoversPage() => NavigateTo<TurnoversPage>("Mijoz hisoboti");
    [RelayCommand] private void ShowSettings() => NavigateTo<SettingsPage>("Sozlamalar");

    private void NavigateTo<T>(string title) where T : notnull
    {
        navigationService.Navigate(services.GetRequiredService<T>());
        CurrentPageTitle = title;
    }
}