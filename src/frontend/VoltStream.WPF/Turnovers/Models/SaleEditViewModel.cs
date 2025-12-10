namespace VoltStream.WPF.Turnovers.Models;

using ApiServices.Extensions;
using ApiServices.Interfaces;
using ApiServices.Models;
using ApiServices.Models.Requests;
using ApiServices.Models.Responses;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using VoltStream.WPF.Commons;
using VoltStream.WPF.Commons.Messages;
using VoltStream.WPF.Commons.Services;
using VoltStream.WPF.Commons.ViewModels;
using VoltStream.WPF.Sales.ViewModels;

public partial class SaleEditViewModel : ViewModelBase
{
    private readonly IMapper mapper;
    private readonly ISaleApi saleApi;
    private readonly ICustomersApi customersApi;
    private readonly ICurrenciesApi currenciesApi;
    private readonly ICategoriesApi categoriesApi;
    private readonly IProductsApi productsApi;
    private readonly IWarehouseStocksApi warehouseStocksApi;
    private readonly INavigationService navigationService;

    private SaleItemViewModel? originalItem;
    private int originalItemIndex = -1;
    private bool _isCalculating = false;
    private bool _suppressLoading = false;

    public SaleEditViewModel(IServiceProvider services, SaleResponse saleData)
    {
        mapper = services.GetRequiredService<IMapper>();
        saleApi = services.GetRequiredService<ISaleApi>();
        customersApi = services.GetRequiredService<ICustomersApi>();
        currenciesApi = services.GetRequiredService<ICurrenciesApi>();
        categoriesApi = services.GetRequiredService<ICategoriesApi>();
        productsApi = services.GetRequiredService<IProductsApi>();
        warehouseStocksApi = services.GetRequiredService<IWarehouseStocksApi>();
        navigationService = services.GetRequiredService<INavigationService>();

        Sale = mapper.Map<SaleViewModel>(saleData);
        Sale.PropertyChanged += SalePropertyChanged;
        CurrentItem.PropertyChanged += CurrentItem_PropertyChanged;

        _ = LoadPageAsync();
        RecalculateTotals();
    }

    [ObservableProperty] private SaleViewModel sale = new();
    [ObservableProperty] private SaleItemViewModel currentItem = new();

    [ObservableProperty] private ObservableCollection<CustomerViewModel> customers = [];
    [ObservableProperty] private ObservableCollection<CurrencyViewModel> currencies = [];
    [ObservableProperty] private ObservableCollection<CategoryViewModel> categories = [];
    [ObservableProperty] private ObservableCollection<Sales.ViewModels.ProductViewModel> products = [];
    [ObservableProperty] private ObservableCollection<WarehouseStockViewModel> warehouseStocks = [];

    [ObservableProperty] private CustomerViewModel? selectedCustomer;
    [ObservableProperty] private CurrencyViewModel? selectedCurrency;
    [ObservableProperty] private CategoryViewModel? selectedCategory;
    [ObservableProperty] private Sales.ViewModels.ProductViewModel? selectedProduct;
    [ObservableProperty] private WarehouseStockViewModel? selectedWarehouseStock;

    [ObservableProperty] private decimal beginBalance;
    [ObservableProperty] private decimal lastBalance;

    [ObservableProperty] private decimal totalSum;

    #region Load Data

    private async Task LoadPageAsync()
    {
        await Task.WhenAll(
            LoadCustomersAsync(),
            LoadCurrenciesAsync(),
            LoadCategoriesAsync(),
            LoadProducts()
        );

        SelectedCustomer = Customers.FirstOrDefault(c => c.Id == Sale.CustomerId);
        SelectedCurrency = Currencies.FirstOrDefault(c => c.Id == Sale.CurrencyId);
    }

    private async Task LoadCustomersAsync()
    {
        FilteringRequest request = new()
        {
            Filters = new() { ["accounts"] = ["include:currency"] }
        };

        var response = await customersApi.FilterAsync(request)
            .Handle(isLoading => IsLoading = isLoading);

        if (response.IsSuccess)
        {
            Customers = mapper.Map<ObservableCollection<CustomerViewModel>>(response.Data!);

            var account = Customers.FirstOrDefault(c => c.Id == Sale.CustomerId)?
                .Accounts.FirstOrDefault(a => a.CurrencyId == Sale.CurrencyId);

            if (account is not null)
            {
                account.Balance += Sale.Amount;

                if (!Sale.IsDiscountApplied)
                {
                    account.Discount -= Sale.Discount;
                    account.Balance += Sale.Discount;
                }
            }
        }
        else
            Error = response.Message ?? "Mijozlarni yuklashda xatolik!";
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
            Currencies = mapper.Map<ObservableCollection<CurrencyViewModel>>(response.Data!);
        else
            Error = response.Message ?? "Valyutalarni yuklashda xatolik!";
    }

    private async Task LoadCategoriesAsync()
    {
        var response = await categoriesApi.GetAllAsync()
            .Handle(isLoading => IsLoading = isLoading);

        if (response.IsSuccess)
            Categories = mapper.Map<ObservableCollection<CategoryViewModel>>(response.Data!);
        else
            Error = response.Message ?? "Kategoriyalarni yuklashda xatolik!";
    }

    #endregion Load Data

    #region Commands

    [RelayCommand]
    private async Task LoadProducts()
    {
        FilteringRequest request = new()
        {
            Filters = new() { ["category"] = ["include"] }
        };

        if (SelectedCategory?.Id > 0)
            request.Filters["CategoryId"] = [SelectedCategory.Id.ToString()];

        var response = await productsApi.Filter(request)
            .Handle(isLoading => IsLoading = isLoading);

        if (response.IsSuccess)
            Products = mapper.Map<ObservableCollection<Sales.ViewModels.ProductViewModel>>(response.Data!);
        else
            Error = response.Message ?? "Maxsulotlarni yuklashda xatolik!";
    }

    [RelayCommand]
    private async Task LoadWarehouseStocks()
    {
        if (SelectedProduct?.Id <= 0) return;

        var response = await warehouseStocksApi.GetProductDetailsFromWarehouseAsync(SelectedProduct.Id)
            .Handle(isLoading => IsLoading = isLoading);

        if (response.IsSuccess)
            WarehouseStocks = mapper.Map<ObservableCollection<WarehouseStockViewModel>>(response.Data!);
        else
            Error = response.Message ?? "Ombor ma'lumotlarini yuklashda xatolik!";
    }

    [RelayCommand]
    private void Add()
    {
        if (SelectedProduct is null)
        {
            Warning = "Maxsulot tanlanmagan!";
            return;
        }

        if (!CurrentItem.TotalLength.HasValue || CurrentItem.TotalLength.Value <= 0)
        {
            Warning = "Miqdor kiritilmagan!";
            return;
        }

        CurrentItem.ProductId = SelectedProduct.Id;
        CurrentItem.Product = new Sales.ViewModels.ProductViewModel
        {
            Id = SelectedProduct.Id,
            Name = SelectedProduct.Name,
            CategoryId = SelectedProduct.CategoryId,
            Category = SelectedProduct.Category is not null ? new CategoryViewModel
            {
                Id = SelectedProduct.Category.Id,
                Name = SelectedProduct.Category.Name
            } : new CategoryViewModel()
        };

        if (IsEditing && originalItem is not null)
        {
            Sale.Items.Insert(originalItemIndex, CurrentItem);
            originalItem = null;
            originalItemIndex = -1;
            IsEditing = false;
        }
        else
        {
            Sale.Items.Insert(0, CurrentItem);
        }

        RecalculateSaleTotals();
        ClearCurrentItem();
    }

    [RelayCommand]
    public async Task EditItem(SaleItemViewModel? item)
    {
        if (item is null) return;

        originalItemIndex = Sale.Items.IndexOf(item);
        originalItem = item;

        CurrentItem = new SaleItemViewModel
        {
            Id = item.Id,
            SaleId = item.SaleId,
            ProductId = item.ProductId,
            RollCount = item.RollCount,
            LengthPerRoll = item.LengthPerRoll,
            TotalLength = item.TotalLength,
            UnitPrice = item.UnitPrice,
            DiscountRate = item.DiscountRate,
            DiscountAmount = item.DiscountAmount,
            TotalAmount = item.TotalAmount,
            FinalAmount = item.FinalAmount,
            Product = item.Product
        };

        IsEditing = true;
        Sale.Items.RemoveAt(originalItemIndex);
        RecalculateSaleTotals();

        _suppressLoading = true;
        try
        {
            if (item.Product?.Category is not null)
            {
                SelectedCategory = Categories.FirstOrDefault(c => c.Id == item.Product.Category.Id);
                await LoadProducts();
            }

            if (item.Product is not null)
            {
                SelectedProduct = Products.FirstOrDefault(p => p.Id == item.ProductId);
                await LoadWarehouseStocks();
            }

            if (item.LengthPerRoll.HasValue)
            {
                SelectedWarehouseStock = WarehouseStocks.FirstOrDefault(w => w.LengthPerRoll == item.LengthPerRoll.Value);
            }
        }
        finally
        {
            _suppressLoading = false;
        }
    }

    [RelayCommand]
    private void CancelEdit()
    {
        if (IsEditing && originalItem is not null)
        {
            Sale.Items.Insert(originalItemIndex, originalItem);
            originalItem = null;
            originalItemIndex = -1;
            IsEditing = false;
            RecalculateSaleTotals();
            ClearCurrentItem();
        }
    }

    [RelayCommand]
    private void DeleteItem(SaleItemViewModel? item)
    {
        if (item is null) return;

        var result = MessageBox.Show(
            "Bu maxsulotni o'chirishni xohlaysizmi?",
            "Tasdiqlash",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            Sale.Items.Remove(item);
            RecalculateSaleTotals();
        }
    }

    [RelayCommand]
    private async Task Save()
    {
        if (Sale.CustomerId <= 0)
        {
            Warning = "Mijoz tanlanmagan!";
            return;
        }

        if (Sale.Items.Count == 0)
        {
            Warning = "Savdo itemlari mavjud emas!";
            return;
        }

        var result = MessageBox.Show(
            "O'zgarishlarni saqlashni xohlaysizmi?",
            "Tasdiqlash",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result != MessageBoxResult.Yes) return;

        var request = new SaleRequest
        {
            Date = Sale.Date,
            CustomerId = Sale.CustomerId,
            CurrencyId = Sale.CurrencyId,
            Amount = Sale.Amount,
            Discount = Sale.Discount,
            Description = Sale.Description,
            Length = Sale.Length,
            RollCount = Sale.RollCount,
            Items = mapper.Map<List<SaleItemRequest>>(Sale.Items)
        };

        var response = await saleApi.Update(request)
            .Handle(isLoading => IsLoading = isLoading);

        if (response.IsSuccess)
        {
            Success = "Savdo muvaffaqiyatli yangilandi!";
            WeakReferenceMessenger.Default.Send(new EntityUpdatedMessage<string>("OperationUpdated"));
            navigationService.GoBack();
        }
        else
        {
            Error = response.Message ?? "Savdoni yangilashda xatolik!";
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
            navigationService.GoBack();
    }

    #endregion Commands

    #region Event Handlers

    partial void OnSelectedCustomerChanged(CustomerViewModel? value)
    {
        if (value is not null)
        {
            Sale.CustomerId = value.Id;
            UpdateCustomerBalance();
        }
    }

    partial void OnSelectedCurrencyChanged(CurrencyViewModel? value)
    {
        if (value is not null)
        {
            Sale.CurrencyId = value.Id;
            UpdateCustomerBalance();
        }
    }

    partial void OnSelectedCategoryChanged(CategoryViewModel? value)
    {
        if (value is not null && !_suppressLoading)
            _ = LoadProducts();
    }

    partial void OnSelectedProductChanged(Sales.ViewModels.ProductViewModel? value)
    {
        if (value is not null && !_suppressLoading)
        {
            if (value.Category is not null)
                SelectedCategory = Categories.FirstOrDefault(c => c.Id == value.Category.Id);

            _ = LoadWarehouseStocks();
        }
    }

    partial void OnSelectedWarehouseStockChanged(WarehouseStockViewModel? value)
    {
        if (value is not null)
        {
            _isCalculating = true;
            CurrentItem.LengthPerRoll = value.LengthPerRoll;
            CurrentItem.UnitPrice = value.UnitPrice;
            CurrentItem.DiscountRate = value.DiscountRate;
            _isCalculating = false;

            // Trigger recalculation
            CalculateFromRollCount();
        }
    }

    partial void OnCurrentItemChanged(SaleItemViewModel? oldValue, SaleItemViewModel newValue)
    {
        if (oldValue is not null)
            oldValue.PropertyChanged -= CurrentItem_PropertyChanged;

        if (newValue is not null)
            newValue.PropertyChanged += CurrentItem_PropertyChanged;
    }

    private void CurrentItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_isCalculating || sender is not SaleItemViewModel item) return;

        _isCalculating = true;
        try
        {
            switch (e.PropertyName)
            {
                case nameof(SaleItemViewModel.RollCount):
                    CalculateFromRollCount();
                    break;

                case nameof(SaleItemViewModel.TotalLength):
                    CalculateFromTotalLength();
                    break;

                case nameof(SaleItemViewModel.UnitPrice):
                    CalculateFromUnitPrice();
                    break;

                case nameof(SaleItemViewModel.TotalAmount):
                    CalculateFromTotalAmount();
                    break;

                case nameof(SaleItemViewModel.DiscountRate):
                    CalculateFromDiscountRate();
                    break;

                case nameof(SaleItemViewModel.DiscountAmount):
                    CalculateFromDiscountAmount();
                    break;

                case nameof(SaleItemViewModel.FinalAmount):
                    CalculateFromFinalAmount();
                    break;
            }
        }
        finally
        {
            _isCalculating = false;
        }
    }

    private void SalePropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(SaleViewModel.IsDiscountApplied))
        {
            RecalculateSaleTotals();
        }
    }

    #endregion Event Handlers

    #region Calculation Methods

    private void CalculateFromRollCount()
    {
        if (!CurrentItem.RollCount.HasValue || !CurrentItem.LengthPerRoll.HasValue)
            return;

        CurrentItem.TotalLength = CurrentItem.RollCount.Value * CurrentItem.LengthPerRoll.Value;

        if (CurrentItem.UnitPrice.HasValue)
        {
            CurrentItem.TotalAmount = CurrentItem.TotalLength.Value * CurrentItem.UnitPrice.Value;

            if (CurrentItem.DiscountRate.HasValue)
            {
                CurrentItem.DiscountAmount = CurrentItem.TotalAmount.Value * (CurrentItem.DiscountRate.Value / 100);
                CurrentItem.FinalAmount = CurrentItem.TotalAmount.Value - CurrentItem.DiscountAmount.Value;
            }
        }
    }

    private async void CalculateFromTotalLength()
    {
        if (!CurrentItem.TotalLength.HasValue || !CurrentItem.LengthPerRoll.HasValue)
            return;

        // Check warehouse stock
        var selectedStock = SelectedWarehouseStock;
        if (selectedStock != null)
        {
            if (CurrentItem.TotalLength.Value > selectedStock.TotalLength)
            {
                Warning = $"Omborda yetarli mahsulot yo'q! Mavjud: {selectedStock.TotalLength:N2} metr";
                return;
            }

            // Check if there's a remainder
            decimal remainder = CurrentItem.TotalLength.Value % CurrentItem.LengthPerRoll.Value;
            if (remainder > 0)
            {
                var result = MessageBox.Show(
                    $"Kiritilgan uzunlik qoldiqli ({remainder:N2} metr). Qoldiq o'lchamida yangi rulon omborga qo'shilsinmi?",
                    "Tasdiqlash",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // TODO: Add new roll to warehouse with remainder length
                    // await AddRollToWarehouse(remainder);
                }
            }
        }

        // Calculate roll count
        if (CurrentItem.LengthPerRoll.Value > 0)
        {
            CurrentItem.RollCount = (int)(CurrentItem.TotalLength.Value / CurrentItem.LengthPerRoll.Value);
        }

        // Calculate amounts
        if (CurrentItem.UnitPrice.HasValue)
        {
            CurrentItem.TotalAmount = CurrentItem.TotalLength.Value * CurrentItem.UnitPrice.Value;

            if (CurrentItem.DiscountRate.HasValue)
            {
                CurrentItem.DiscountAmount = CurrentItem.TotalAmount.Value * (CurrentItem.DiscountRate.Value / 100);
                CurrentItem.FinalAmount = CurrentItem.TotalAmount.Value - CurrentItem.DiscountAmount.Value;
            }
        }
    }

    private void CalculateFromUnitPrice()
    {
        if (!CurrentItem.UnitPrice.HasValue || !CurrentItem.TotalLength.HasValue)
            return;

        CurrentItem.TotalAmount = CurrentItem.TotalLength.Value * CurrentItem.UnitPrice.Value;

        if (CurrentItem.DiscountRate.HasValue)
        {
            CurrentItem.DiscountAmount = CurrentItem.TotalAmount.Value * (CurrentItem.DiscountRate.Value / 100);
            CurrentItem.FinalAmount = CurrentItem.TotalAmount.Value - CurrentItem.DiscountAmount.Value;
        }
    }

    private void CalculateFromTotalAmount()
    {
        if (!CurrentItem.TotalAmount.HasValue || !CurrentItem.TotalLength.HasValue)
            return;

        // Recalculate unit price
        if (CurrentItem.TotalLength.Value > 0)
        {
            CurrentItem.UnitPrice = CurrentItem.TotalAmount.Value / CurrentItem.TotalLength.Value;
        }

        // Recalculate discount
        if (CurrentItem.DiscountRate.HasValue)
        {
            CurrentItem.DiscountAmount = CurrentItem.TotalAmount.Value * (CurrentItem.DiscountRate.Value / 100);
            CurrentItem.FinalAmount = CurrentItem.TotalAmount.Value - CurrentItem.DiscountAmount.Value;
        }
    }

    private void CalculateFromDiscountRate()
    {
        if (!CurrentItem.DiscountRate.HasValue || !CurrentItem.TotalAmount.HasValue)
            return;

        // Ensure discount rate doesn't exceed 100%
        if (CurrentItem.DiscountRate.Value > 100)
        {
            CurrentItem.DiscountRate = 100;
            Warning = "Chegirma 100% dan oshishi mumkin emas!";
        }

        CurrentItem.DiscountAmount = CurrentItem.TotalAmount.Value * (CurrentItem.DiscountRate.Value / 100);
        CurrentItem.FinalAmount = CurrentItem.TotalAmount.Value - CurrentItem.DiscountAmount.Value;
    }

    private void CalculateFromDiscountAmount()
    {
        if (!CurrentItem.DiscountAmount.HasValue || !CurrentItem.TotalAmount.HasValue)
            return;

        // Ensure discount doesn't exceed total amount
        if (CurrentItem.DiscountAmount.Value > CurrentItem.TotalAmount.Value)
        {
            CurrentItem.DiscountAmount = CurrentItem.TotalAmount.Value;
            Warning = "Chegirma summasi jami summadan oshishi mumkin emas!";
        }

        // Calculate discount rate
        if (CurrentItem.TotalAmount.Value > 0)
        {
            CurrentItem.DiscountRate = (CurrentItem.DiscountAmount.Value / CurrentItem.TotalAmount.Value) * 100;
        }

        CurrentItem.FinalAmount = CurrentItem.TotalAmount.Value - CurrentItem.DiscountAmount.Value;
    }

    private void CalculateFromFinalAmount()
    {
        if (!CurrentItem.FinalAmount.HasValue || !CurrentItem.TotalAmount.HasValue)
            return;

        // Calculate discount amount
        CurrentItem.DiscountAmount = CurrentItem.TotalAmount.Value - CurrentItem.FinalAmount.Value;

        // Ensure discount is not negative
        if (CurrentItem.DiscountAmount.Value < 0)
        {
            CurrentItem.DiscountAmount = 0;
            CurrentItem.FinalAmount = CurrentItem.TotalAmount.Value;
            Warning = "Umumiy summa jami summadan katta bo'lishi mumkin emas!";
        }

        // Calculate discount rate
        if (CurrentItem.TotalAmount.Value > 0)
        {
            CurrentItem.DiscountRate = (CurrentItem.DiscountAmount.Value / CurrentItem.TotalAmount.Value) * 100;

            // Ensure discount rate doesn't exceed 100%
            if (CurrentItem.DiscountRate.Value > 100)
            {
                CurrentItem.DiscountRate = 100;
                CurrentItem.DiscountAmount = CurrentItem.TotalAmount.Value;
                CurrentItem.FinalAmount = 0;
                Warning = "Chegirma 100% dan oshishi mumkin emas!";
            }
        }
    }

    private void RecalculateSaleTotals()
    {
        if (Sale.Items.Count == 0)
        {
            Sale.Amount = 0;
            Sale.Discount = 0;
            TotalSum = 0;
            Sale.Discount = 0;
            Sale.Amount = 0;
        }
        else
        {
            var grossAmount = Sale.Items.Sum(x => x.TotalAmount ?? 0);
            var discountAmount = Sale.Items.Sum(x => x.DiscountAmount ?? 0);

            TotalSum = grossAmount;
            Sale.Discount = discountAmount;

            Sale.Amount = grossAmount;
            if (Sale.IsDiscountApplied)
                Sale.Amount -= discountAmount;

            Sale.RollCount = Sale.Items.Sum(x => x.RollCount ?? 0);
            Sale.Length = Sale.Items.Sum(x => x.TotalLength ?? 0);
        }

        UpdateCustomerBalance();
    }

    private void RecalculateTotals()
    {
        RecalculateSaleTotals();
    }

    #endregion Calculation Methods

    #region Helpers

    private void UpdateCustomerBalance()
    {
        if (SelectedCustomer is not null && SelectedCurrency is not null)
        {
            var account = SelectedCustomer.Accounts.FirstOrDefault(a => a.CurrencyId == SelectedCurrency.Id);

            if (account is not null)
            {
                BeginBalance = account.Balance;
                LastBalance = BeginBalance - Sale.Amount;
            }
            else
            {
                var rate = SelectedCurrency.ExchangeRate > 0 ? SelectedCurrency.ExchangeRate : 1;
                BeginBalance = SelectedCustomer.Balance / rate;
                LastBalance = BeginBalance - Sale.Amount;
            }
        }
        else
        {
            BeginBalance = 0;
            LastBalance = 0;
        }
    }

    private void ClearCurrentItem()
    {
        CurrentItem = new SaleItemViewModel();
        SelectedCategory = null;
        SelectedProduct = null;
        SelectedWarehouseStock = null;
    }

    #endregion Helpers
}

public partial class SaleItemForEntryRow : ViewModelBase
{
    public long Id { get; set; }
    public long SaleId { get; set; }
    public long ProductId { get; set; }
    [ObservableProperty] private string rollCount = string.Empty;
    [ObservableProperty] private string lengthPerRoll = string.Empty;
    [ObservableProperty] private string totalLength = string.Empty;
    [ObservableProperty] private string unitPrice = string.Empty;
    [ObservableProperty] private string discountRate = string.Empty;
    [ObservableProperty] private string discountAmount = string.Empty;
    [ObservableProperty] private string totalAmount = string.Empty;
    [ObservableProperty] private string finalAmount = string.Empty;

    [ObservableProperty] private SaleViewModel sale = new();
    [ObservableProperty] private Commons.ViewModels.ProductViewModel product = new();
}