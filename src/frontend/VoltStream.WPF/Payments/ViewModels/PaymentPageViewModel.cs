namespace VoltStream.WPF.Payments.ViewModels;

using ApiServices.Extensions;
using ApiServices.Interfaces;
using ApiServices.Models;
using ApiServices.Models.Requests;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using VoltStream.WPF.Commons;
using VoltStream.WPF.Commons.Messages; // OpenDialogMessage<T> ni o'z ichiga oladi
using VoltStream.WPF.Commons.ViewModels;
using VoltStream.WPF.Payments.PayDiscountWindow.Modela;

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
        paymentApi = services.GetRequiredService<IPaymentApi>();
        mapper = services.GetRequiredService<IMapper>();

        Payment = new();
        Payment.PropertyChanged += Payment_PropertyChanged;

        _ = LoadDataAsync();
    }

    [ObservableProperty] private ObservableCollection<PaymentViewModel> historyPayments = [];
    [ObservableProperty] private ObservableCollection<PaymentViewModel> availablePayments = [];
    [ObservableProperty] private ObservableCollection<CurrencyViewModel> availableCurrencies = [];
    [ObservableProperty] private ObservableCollection<CustomerViewModel> availableCustomers = [];
    [ObservableProperty] private PaymentViewModel payment = new();
    [ObservableProperty] private CustomerViewModel? customer;

    private long customerId;

    private async void Payment_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(Payment.PaidAt))
        {
            await LoadDatagrid();
        }
    }

    #region Commands

    [RelayCommand]
    private async Task Submit()
    {
        if (Customer is null)
        {
            Warning = "To'lov amalga oshirilayotgan shaxs tanlanishi shart";
            return;
        }

        var request = mapper.Map<PaymentRequest>(Payment);
        request.CustomerId = Customer.Id;

        var response = await paymentApi.CreateAsync(request).Handle(isLoading => IsLoading = isLoading);

        if (response.IsSuccess)
        {
            Success = "To'lov muvaffaqiyatli ro'yxatga qo'shildi!";
            DateTime date = Payment.PaidAt;
            Payment = new();
            Payment.PropertyChanged += Payment_PropertyChanged;
            Customer = new();
            Payment.PaidAt = date;
            await LoadDataAsync();
        }
        else Error = response.Message ?? "Texnik xatolik!";
    }

    [RelayCommand]
    private Task OpenDiscountsWindow()
    {
        if (Customer is null)
        {
            Warning = "Mijoz tanlanishi shart!";
            return Task.CompletedTask;
        }

        if (Payment.Discount is null || Payment.Discount <= 0)
        {
            Warning = "Mijoz uchun mavjud chegirma yo'q!";
            return Task.CompletedTask;
        }

        var discountData = new PayDiscountData(customerId, Customer.Name, Payment.Discount.Value, Payment.PaidAt);

        WeakReferenceMessenger.Default.Send(new OpenDialogMessage<PayDiscountData> { ViewModelData = discountData });

        return Task.CompletedTask;
    }

    public async Task ApplyDiscountResultAsync(dynamic? result)
    {
        if (result is null) return;

        var request = new ApplyDiscountRequest
        {
            Date = Payment.PaidAt,
            CustomerId = customerId,
            DiscountAmount = result!.discountSum,
            IsCash = result.discountCash,
            Description = result.discountInfo ?? string.Empty
        };

        var response = await paymentApi.ApplyAsync(request)
            .Handle(isLoading => IsLoading = isLoading);

        if (response.IsSuccess)
        {
            Success = "Chegirma muvaffaqiyatli qo'llandi!";
            await LoadCustomerAsync();

            Success = $"Chegirma muvaffaqiyatli qo'llandi!\n\n" +
                $"Turi: {(result.discountCash ? "Naqd to'lov (kassadan)" : "Hisobga qo'shildi")}\n" +
                $"Summa: {result.discountSum:N2}\n" +
                $"Izoh: {result.discountInfo}\n" +
                $"Operatsiya ID: {response.Data}";
        }
        else
        {
            Error = response.Message ?? "Chegirma qo'llashda xatolik!";
        }
    }

    #endregion Commands

    #region Load data

    private async Task LoadDataAsync()
    {
        await LoadCustomersAsync();
        await LoadCurrenciesAsync();
        await LoadDatagrid();
    }

    private async Task LoadDatagrid()
    {
        string date = Payment.PaidAt.ToString("dd.MM.yyyy");
        FilteringRequest request = new()
        {
            Filters = new()
            {
                ["paidAt"] = [date],
                ["customer"] = ["include"],
                ["currency"] = ["include"]
            }
        };

        var response = await paymentApi.FilterAsync(request).Handle(isLoading => IsLoading = isLoading);

        if (response.IsSuccess)
            HistoryPayments = mapper.Map<ObservableCollection<PaymentViewModel>>(response.Data);
        else
            Error = response.Message ?? "To'lovlarni olishda xatolik!";
    }

    private async Task LoadCurrenciesAsync()
    {
        FilteringRequest request = new()
        {
            Filters = new()
            {
                ["isactive"] = ["true"]
            }
        };

        var response = await currenciesApi.Filter(request).Handle(isLoading => IsLoading = isLoading);

        if (response.IsSuccess)
        {
            AvailableCurrencies = mapper.Map<ObservableCollection<CurrencyViewModel>>(response.Data);
            Payment.Currency = AvailableCurrencies.FirstOrDefault(c => c.IsDefault)!;
        }
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

    partial void OnCustomerChanged(CustomerViewModel? value)
    {
        if (customer is null || customerId == value?.Id)
            return;

        customerId = value!.Id;
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

        var response = await customersApi.FilterAsync(request).Handle();

        if (response.IsSuccess)
        {
            Customer = mapper.Map<CustomerViewModel>(response.Data.FirstOrDefault() ?? new());

            if (Customer.Accounts is not null)
            {
                foreach (var account in Customer.Accounts)
                    if (account.Currency is not null && account.Currency.Code == "UZS")
                    {
                        Payment.Balance = account.Balance;
                        Payment.Discount = account.Discount;
                    }

                Payment.ReCalculateIncome();
                Payment.ReCalculateExpense();
            }
        }
        else Error = response.Message ?? "Mijoz ma'lumotlarnini yuklashda xatolik!";
    }

    #endregion Load data
}