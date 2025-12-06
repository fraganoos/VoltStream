namespace VoltStream.WPF.Payments.ViewModels;

using ApiServices.Extensions;
using ApiServices.Interfaces;
using ApiServices.Models;
using ApiServices.Models.Requests;
using ApiServices.Models.Responses;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using VoltStream.WPF.Commons;
using VoltStream.WPF.Commons.ViewModels;

public partial class PaymentEditViewModel : ViewModelBase
{
    private readonly IServiceProvider services;
    private readonly IMapper mapper;
    private readonly IPaymentApi paymentApi;
    private readonly ICustomersApi customersApi;
    private readonly ICurrenciesApi currenciesApi;

    public event EventHandler<bool>? CloseRequested;

    public PaymentEditViewModel(IServiceProvider services, PaymentResponse paymentData)
    {
        this.services = services;
        mapper = services.GetRequiredService<IMapper>();
        paymentApi = services.GetRequiredService<IPaymentApi>();
        customersApi = services.GetRequiredService<ICustomersApi>();
        currenciesApi = services.GetRequiredService<ICurrenciesApi>();

        // Ma'lumotlarni to'g'ridan-to'g'ri o'zlashtiramiz
        Payment = mapper.Map<PaymentViewModel>(paymentData);

        // Kirim yoki chiqimni aniqlash
        if (Payment.Amount > 0)
            Payment.IncomeAmount = Payment.NetAmount;
        else if (Payment.Amount < 0)
            Payment.ExpenseAmount = Math.Abs(Payment.NetAmount);

        Payment.PropertyChanged += Payment_PropertyChanged;

        _ = LoadPageAsync();
    }

    [ObservableProperty] private PaymentViewModel payment = new();
    [ObservableProperty] private decimal? beginBalance;
    [ObservableProperty] private decimal? lastBalance;

    [ObservableProperty] private ObservableCollection<CustomerViewModel> customers = [];
    [ObservableProperty] private ObservableCollection<CurrencyViewModel> currencies = [];

    private async void Payment_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Payment.IncomeAmount) ||
            e.PropertyName == nameof(Payment.ExpenseAmount))
        {
            CalculateLastBalance();
        }
    }

    private async Task LoadPageAsync()
    {
        await Task.WhenAll(
            LoadCustomersAsync(),
            LoadCurrenciesAsync(),
            LoadCustomerBalance()
        );
    }

    private async Task LoadCustomersAsync()
    {
        var response = await customersApi.GetAllAsync()
            .Handle(isLoading => IsLoading = isLoading);

        if (response.IsSuccess)
        {
            Customers = mapper.Map<ObservableCollection<CustomerViewModel>>(response.Data!);

            if (Payment.CustomerId > 0)
            {
                var currentCustomer = Customers.FirstOrDefault(c => c.Id == Payment.CustomerId);
                if (currentCustomer is not null)
                    Payment.Customer = currentCustomer;
            }
        }
        else Error = response.Message ?? "Mijozlarni yuklashda xatolik!";
    }

    private async Task LoadCurrenciesAsync()
    {
        FilteringRequest request = new()
        {
            Filters = new() { ["isactive"] = ["true"] }
        };

        var response = await currenciesApi.Filter(request)
            .Handle(isLoading => IsLoading = isLoading);

        if (response.IsSuccess)
        {
            Currencies = mapper.Map<ObservableCollection<CurrencyViewModel>>(response.Data!);

            // Joriy valyutani topib belgilaymiz
            if (Payment.CurrencyId > 0)
            {
                var currentCurrency = Currencies.FirstOrDefault(c => c.Id == Payment.CurrencyId);
                if (currentCurrency is not null)
                    Payment.Currency = currentCurrency;
            }
        }
        else Error = response.Message ?? "Valyutalarni yuklashda xatolik!";
    }

    private async Task LoadCustomerBalance()
    {
        if (Payment.Customer is null) return;

        FilteringRequest request = new()
        {
            Filters = new()
            {
                ["id"] = [Payment.Customer.Id.ToString()],
                ["accounts"] = ["include:currency"]
            }
        };

        var response = await customersApi.Filter(request)
            .Handle(isLoading => IsLoading = isLoading);

        if (response.IsSuccess)
        {
            var customer = response.Data.First();
            if (customer.Accounts is not null)
            {
                var uzsAccount = customer.Accounts.FirstOrDefault(a => a.Currency?.Code == "UZS");
                if (uzsAccount is not null)
                {
                    BeginBalance = uzsAccount.Balance;
                    CalculateLastBalance();
                }
            }
        }
    }

    private void CalculateLastBalance()
    {
            LastBalance = BeginBalance + Payment.Amount;
    }

    [RelayCommand]
    private async Task Save()
    {
        if (Payment.Customer is null)
        {
            Warning = "Mijoz tanlanmagan!";
            return;
        }

        if (!Payment.IncomeAmount.HasValue && !Payment.ExpenseAmount.HasValue)
        {
            Warning = "Kirim yoki chiqim summasi kiritilishi shart!";
            return;
        }

        var result = MessageBox.Show(
            "O'zgarishlarni saqlashni xohlaysizmi?",
            "Tasdiqlash",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes)
            return;

        try
        {
            var request = new PaymentRequest
            {
                Id = Payment.Id,
                CustomerId = Payment.Customer.Id,
                CurrencyId = Payment.Currency?.Id ?? 0,
                ExchangeRate = Payment.ExchangeRate,
                Amount = Payment.Amount,
                PaidAt = Payment.PaidAt,
                Description = Payment.Description
            };

            var response = await paymentApi.UpdateAsync(request)
                .Handle(isLoading => IsLoading = isLoading);

            if (response.IsSuccess)
            {
                Success = "To'lov muvaffaqiyatli yangilandi!";
                CloseRequested?.Invoke(this, true);
            }
            else
            {
                Error = response.Message ?? "To'lovni yangilashda xatolik!";
            }
        }
        catch (Exception ex)
        {
            Error = $"Xatolik: {ex.Message}";
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        var result = MessageBox.Show(
            "O'zgarishlar saqlanmaydi. Chiqishni xohlaysizmi?",
            "Tasdiqlash",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
            CloseRequested?.Invoke(this, false);
    }
}