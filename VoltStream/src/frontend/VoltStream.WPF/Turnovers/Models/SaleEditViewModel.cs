namespace VoltStream.WPF.Sales.ViewModels;

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
using System.Windows;
using VoltStream.WPF.Commons;
using VoltStream.WPF.Commons.ViewModels;
using VoltStream.WPF.Turnovers.Models;

public partial class SaleEditViewModel : ViewModelBase
{
    private readonly IServiceProvider services;
    private readonly IMapper mapper;
    private readonly ISaleApi saleApi;
    private readonly ICustomersApi customersApi;
    private readonly ICurrenciesApi currenciesApi;
    private readonly ICategoriesApi categoriesApi;

    public event EventHandler<bool>? CloseRequested;

    public SaleEditViewModel(IServiceProvider services, SaleResponse saleData)
    {
        this.services = services;
        mapper = services.GetRequiredService<IMapper>();
        saleApi = services.GetRequiredService<ISaleApi>();
        customersApi = services.GetRequiredService<ICustomersApi>();
        currenciesApi = services.GetRequiredService<ICurrenciesApi>();
        categoriesApi = services.GetRequiredService<ICategoriesApi>();

        Sale = mapper.Map<SaleViewModel>(saleData);

        _ = LoadPageAsync();
        CalculateTotals();
    }

    [ObservableProperty] private SaleViewModel sale;
    [ObservableProperty] private SaleItemViewModel currentItem = new();
    [ObservableProperty] private decimal totalSum;
    [ObservableProperty] private decimal totalDiscount;
    [ObservableProperty] private decimal finalSum;
    [ObservableProperty] private decimal? beginBalance;
    [ObservableProperty] private decimal? lastBalance;

    [ObservableProperty] private ObservableCollection<CustomerViewModel> customers = [];
    [ObservableProperty] private ObservableCollection<CurrencyViewModel> currencies = [];
    [ObservableProperty] private ObservableCollection<CategoryViewModel> categories = [];

    private async Task LoadPageAsync()
    {
        await LoadCustomersAsync();
        await LoadCurrenciesAsync();
        await LoadCategoriesAsync();
        await LoadCustomerBalance();
    }

    private async Task LoadCustomersAsync()
    {
        var response = await customersApi.GetAllAsync().Handle(isLoading => IsLoading = isLoading);
        if (response.IsSuccess)
            Customers = mapper.Map<ObservableCollection<CustomerViewModel>>(response.Data!);
        else
            Error = response.Message ?? "Mijozlarni yuklashda xatolik!";
    }

    private async Task LoadCurrenciesAsync()
    {
        FilteringRequest request = new()
        {
            Filters = new() { ["isactive"] = ["true"] }
        };

        var response = await currenciesApi.Filter(request).Handle(isLoading => IsLoading = isLoading);
        if (response.IsSuccess)
            Currencies = mapper.Map<ObservableCollection<CurrencyViewModel>>(response.Data!);
        else
            Error = response.Message ?? "Valyutalarni yuklashda xatolik!";
    }

    private async Task LoadCategoriesAsync()
    {
        var request = new FilteringRequest
        {
            Filters = new() { ["products"] = ["include"] }
        };

        var response = await categoriesApi.Filter(request).Handle(isLoading => IsLoading = isLoading);
        if (response.IsSuccess)
            Categories = mapper.Map<ObservableCollection<CategoryViewModel>>(response.Data!);
        else
            Error = response.Message ?? "Kategoriyalarni yuklashda xatolik!";
    }

    private async Task LoadCustomerBalance()
    {
        if (Sale.Customer == null) return;

        FilteringRequest request = new()
        {
            Filters = new()
            {
                ["id"] = [Sale.Customer.Id.ToString()],
                ["accounts"] = ["include:currency"]
            }
        };

        var response = await customersApi.Filter(request).Handle();
        if (response.IsSuccess && response.Data.Any())
        {
            var customer = response.Data.First();
            if (customer.Accounts != null)
            {
                var uzsAccount = customer.Accounts.FirstOrDefault(a => a.Currency?.Code == "UZS");
                if (uzsAccount != null)
                {
                    BeginBalance = uzsAccount.Balance;
                    LastBalance = BeginBalance + FinalSum;
                }
            }
        }
    }

    [RelayCommand]
    private void Add()
    {
        if (CurrentItem == null || string.IsNullOrWhiteSpace(CurrentItem.Product.Name))
        {
            Warning = "Maxsulot ma'lumotlari to'liq emas!";
            return;
        }

        Sale.Items.Add(CurrentItem);
        CurrentItem = new SaleItemViewModel();
        CalculateTotals();
    }

    [RelayCommand]
    private async Task Save()
    {
        if (Sale.Customer == null)
        {
            Warning = "Mijoz tanlanmagan!";
            return;
        }

        if (Sale.Items == null || !Sale.Items.Any())
        {
            Warning = "Savdo itemlari mavjud emas!";
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
            var request = new SaleRequest
            {
                Id = Sale.Id,
                Date = Sale.Date,
                CustomerId = Sale.Customer.Id,
                CurrencyId = Sale.Currency?.Id ?? 0,
                Description = Sale.Description,
                Items = Sale.Items.Select(item => new SaleItemRequest
                {
                    ProductId = item.ProductId,
                    //Quantity = item.Quantity,
                    //Price = item.Price,
                    //Discount = item.Discount
                }).ToList()
            };

            var response = await saleApi.Update(request)
                .Handle(isLoading => IsLoading = isLoading);

            if (response.IsSuccess)
            {
                Success = "Savdo muvaffaqiyatli yangilandi!";
                CloseRequested?.Invoke(this, true);
            }
            else
            {
                Error = response.Message ?? "Savdoni yangilashda xatolik!";
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

    private void CalculateTotals()
    {
        if (Sale.Items == null || !Sale.Items.Any())
        {
            TotalSum = 0;
            TotalDiscount = 0;
            FinalSum = 0;
            return;
        }

        //TotalSum = Sale.Items.Sum(x => x.Sum);
        //TotalDiscount = Sale.Items.Sum(x => x.Discount);
        FinalSum = TotalSum - TotalDiscount;

        if (BeginBalance.HasValue)
            LastBalance = BeginBalance + FinalSum;
    }

    partial void OnSaleChanged(SaleViewModel value)
    {
        CalculateTotals();
    }
}