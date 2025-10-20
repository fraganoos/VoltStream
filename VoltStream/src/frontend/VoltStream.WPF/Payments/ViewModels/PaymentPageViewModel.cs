namespace VoltStream.WPF.Payments.ViewModels;

using ApiServices.Extensions;
using ApiServices.Interfaces;
using ApiServices.Models;
using ApiServices.Models.Requests;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
    public readonly IPaymentApi paymentApi;
    public readonly IMapper mapper;
    public PaymentPageViewModel(IServiceProvider services)
    {
        customersApi = services.GetRequiredService<ICustomersApi>();
        currenciesApi = services.GetRequiredService<ICurrenciesApi>();
        mapper = services.GetRequiredService<IMapper>();
        paymentApi = services.GetRequiredService<IPaymentApi>();

        _ = LoadDataAsync();
    }


    [ObservableProperty] private ObservableCollection<PaymentViewModel> availablePayments = [];
    [ObservableProperty] private ObservableCollection<CurrencyViewModel> availableCurrencies = [];
    [ObservableProperty] private ObservableCollection<CustomerViewModel> availableCustomers = [];

    [ObservableProperty] private PaymentViewModel payment = new();
    [ObservableProperty] private CustomerViewModel? customer;
    private long customerId;

    [ObservableProperty] private CurrencyViewModel? currency;

    #region Commands

    [RelayCommand]
    private async Task Submit()
    {
        var request = new PaymentRequest
        {
            CustomerId = Customer.Id,
            CurrencyId = Currency.Id,
            PaidAt = Payment.PaidAt,
            Amount = Payment.Amount,
            ExchangeRate = Payment.ExchangeRate,
            NetAmount = Payment.NetAmount,
            Description = Payment.Description,
        };

        var response = await paymentApi.CreateAsync(request).Handle();

        if (response.IsSuccess)
        {
            Success = "To'lov muvaffaqiyatli ro'yxatga qo'shildi!";

            Payment = new();
            Customer = new();
            Currency = new();
        }
        else Error = response.Message ?? "Texnik xatolik!";
    }

    #endregion Commands


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
        {
            Customer = mapper.Map<CustomerViewModel>(response.Data.FirstOrDefault() ?? new());
            if (Customer.Accounts is not null)
                foreach (var account in Customer.Accounts)
                    if (account.Currency is not null && account.Currency.Code == "UZS")
                        Payment.Balance = account.Balance;
        }
        else Error = response.Message ?? "Mijoz ma'lumotlarnini yuklashda xatolik!";
    }

    #endregion Load data
}
