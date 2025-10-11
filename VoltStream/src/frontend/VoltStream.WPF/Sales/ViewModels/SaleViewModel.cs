namespace VoltStream.WPF.Sales.ViewModels;

using ApiServices.Extensions;
using ApiServices.Interfaces;
using ApiServices.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using VoltStream.WPF.Commons;
using VoltStream.WPF.Customer.ViewModels;
using VoltStream.WPF.Payments.ViewModels;

public partial class SaleViewModel : ViewModelBase
{
    private readonly IMapper mapper;
    private readonly ICurrenciesApi currenciesApi;
    private readonly ICustomersApi customersApi;
    private readonly ICategoriesApi categoriesApi;
    private readonly IProductsApi productsApi;
    private readonly IWarehouseStocksApi stocksApi;

    public SaleViewModel(IServiceProvider services)
    {
        mapper = services.GetRequiredService<IMapper>();
        currenciesApi = services.GetRequiredService<ICurrenciesApi>();
        customersApi = services.GetRequiredService<ICustomersApi>();
        categoriesApi = services.GetRequiredService<ICategoriesApi>();
        productsApi = services.GetRequiredService<IProductsApi>();
        stocksApi = services.GetRequiredService<IWarehouseStocksApi>();

        _ = LoadPageAsync();
    }

    private async Task LoadPageAsync()
    {
        await LoadCurrenciesAsync();
        await LoadCategoryAndProductsAsync();
        await LoadCustomersAsync();
    }

    [ObservableProperty] private long id;
    [ObservableProperty] private DateTime date = DateTime.Now;
    [ObservableProperty] private decimal? amount;
    [ObservableProperty] private int rollCount;
    [ObservableProperty] private decimal length;
    [ObservableProperty] private string description = string.Empty;
    [ObservableProperty] private decimal? finalSum;

    [ObservableProperty] private long customerOperationId;
    [ObservableProperty] private CustomerOperationViewModel customerOperation = new();

    [ObservableProperty] private decimal? discount;
    [ObservableProperty] private bool isApplied;
    [ObservableProperty] private DiscountOperationViewModel discountOperation = new();

    [ObservableProperty] private long currencyId;
    [ObservableProperty] private CurrencyViewModel currency = new();

    [ObservableProperty] private long customerId;
    [ObservableProperty] private CustomerViewModel customer = new();

    [ObservableProperty] private ObservableCollection<SaleItemViewModel> items = [];

    // ============ for UI ================

    #region Commands

    [ObservableProperty] private SaleItemViewModel currentItem = new();
    [RelayCommand]
    public void Add()
    {
        Items.Add(CurrentItem);
        CurrentItem = new();
    }

    #endregion Commands

    #region Currencies combobox

    [ObservableProperty] private ObservableCollection<CurrencyViewModel> currencies = [];

    private async Task LoadCurrenciesAsync()
    {
        var response = await currenciesApi.GetAllAsync().Handle();
        if (response.IsSuccess)
        {
            Currencies = new(mapper.Map<ObservableCollection<CurrencyViewModel>>(response.Data!));
            if (CurrencyId > 0)
                Currency = Currencies.FirstOrDefault(c => c.Id == CurrencyId)!;
        }
        else Error = response.Message;
    }

    partial void OnCurrencyChanged(CurrencyViewModel value)
    {
        CurrencyId = value.Id;
    }

    #endregion Currencies combobox

    #region Customer combobox

    [ObservableProperty] private ObservableCollection<CustomerViewModel> customers = [];

    private async Task LoadCustomersAsync()
    {
        FilteringRequest request = new()
        {
            Filters = new()
            {
                ["Accounts"] = ["include:Currency"]
            }
        };

        var response = await customersApi.Filter(request).Handle();
        if (response.IsSuccess)
        {
            Customers = new(mapper.Map<ObservableCollection<CustomerViewModel>>(response.Data!));
            if (CustomerId > 0)
                Customer = Customers.FirstOrDefault(c => c.Id == CustomerId)!;
        }
        else Error = response.Message;
    }

    #endregion Customer combobox

    #region Categories combobox

    [ObservableProperty] private ObservableCollection<CategoryViewModel> categories = [];
    [ObservableProperty] private CategoryViewModel? selectedCategory;
    [ObservableProperty] private long? selectedCategoryId;
    partial void OnSelectedCategoryIdChanged(long? value)
    {
        RefreshProducts();
    }

    partial void OnSelectedCategoryChanged(CategoryViewModel? value)
    {
        selectedCategoryId = value?.Id;
        RefreshProducts();
    }

    private async Task LoadCategoryAndProductsAsync()
    {
        var request = new FilteringRequest
        {
            Filters = new()
            {
                ["products"] = ["include"]
            }
        };

        var response = await categoriesApi.Filter(request).Handle();
        if (!response.IsSuccess)
        {
            Error = response.Message;
            return;
        }

        var mapped = mapper.Map<List<CategoryViewModel>>(response.Data!);

        Categories = new ObservableCollection<CategoryViewModel>(mapped);
        if (SelectedCategoryId > 0)
            SelectedCategory = mapped.FirstOrDefault(c => c.Id == SelectedCategoryId);
        RefreshProducts();
    }

    #endregion Categories combobox

    #region Products combobox

    [ObservableProperty] private ObservableCollection<ProductViewModel> products = [];

    private void RefreshProducts()
    {
        if (SelectedCategoryId > 0)
        {
            if (SelectedCategory is null)
            {
                Products = mapper.Map<ObservableCollection<ProductViewModel>>(Categories.SelectMany(c => c.Products));
                Error = "Bunday category mavjud emas yoki o'chirilgan";
            }
            else
                Products = SelectedCategory.Products;
        }
        else Products = mapper.Map<ObservableCollection<ProductViewModel>>(Categories.SelectMany(c => c.Products));
    }

    #endregion Products combobox

    #region Stocks

    [ObservableProperty] private ObservableCollection<WarehouseStockViewModel> stocks = [];
    private async Task LoadStocks()
    {
        FilteringRequest request = new()
        {
            Filters = new()
            {
                ["productId"] = [CurrentItem.Id.ToString()]
            }
        };

        var response = await stocksApi.Filter(request).Handle();
        if (response.IsSuccess)
        {
            Stocks = new(mapper.Map<ObservableCollection<WarehouseStockViewModel>>(response.Data!));
        }
        else Error = response.Message;
    }

    #endregion Stocks
}
