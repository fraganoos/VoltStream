namespace VoltStream.WPF.Payments.ViewModels;

using ApiServices.Extensions;
using ApiServices.Interfaces;
using ApiServices.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using VoltStream.WPF.Commons;
using VoltStream.WPF.Commons.ViewModels;

partial class PaymentPageViewModel : ViewModelBase
{
    public readonly ICustomersApi customersApi;
    public readonly ICurrenciesApi currenciesApi;
    public readonly IMapper mapper;
    public PaymentPageViewModel(IServiceProvider services)
    {
        customersApi = services.GetRequiredService<ICustomersApi>();
        currenciesApi = services.GetRequiredService<ICurrenciesApi>();
        mapper = services.GetRequiredService<IMapper>();

        _ = LoadDataAsync();
    }


    [ObservableProperty] private ObservableCollection<PaymentViewModel> availablePayments = [];
    [ObservableProperty] private ObservableCollection<CurrencyViewModel> availableCurrencies = [];
    [ObservableProperty] private ObservableCollection<CustomerViewModel> availableCustomers = [];

    [ObservableProperty] private CurrencyViewModel currency;
    [ObservableProperty] private CustomerViewModel customer;
    private long customerId;






    #region Load data

    private async Task LoadDataAsync()
    {
        await LoadCustomersAsync();
        await LoadCurrenciesAsync();
    }

    private async Task LoadCurrenciesAsync()
    {
        var response = await currenciesApi.GetAllAsync().Handle();
        if (response.IsSuccess)
            AvailableCurrencies = mapper.Map<ObservableCollection<CurrencyViewModel>>(response.Data);
        else
            Error = response.Message ?? "Valyuteni yuklashda xatolik!";
    }

    public async Task LoadCustomersAsync()
    {
        var response = await customersApi.GetAllAsync().Handle();
        if (response.IsSuccess)
            AvailableCustomers = mapper.Map<ObservableCollection<CustomerViewModel>>(response.Data);
        else
            Error = response.Message ?? "Valyuteni yuklashda xatolik!";
    }

    // tanlangan customer ma'lumotlarini yuklash
    partial void OnCustomerChanged(CustomerViewModel value)
    {
        if (value is null || customerId == value.Id)
            return;

        customerId = value.Id;
        _ = LoadCustomerAsync();
    }

    private async Task LoadCustomerAsync()
    {
        FilteringRequest request = new()
        {
            Filters = new()
            {
                ["id"] = [customerId.ToString()],
                ["accounts"] = ["include:currency"]
            }
        };

        var response = await customersApi.Filter(request).Handle();

        if (response.IsSuccess)
            Customer = mapper.Map<CustomerViewModel>(response.Data.FirstOrDefault() ?? new());
        else Error = response.Message ?? "Mijoz ma'lumotlarnini yuklashda xatolik!";
    }

    #endregion Load data
}
