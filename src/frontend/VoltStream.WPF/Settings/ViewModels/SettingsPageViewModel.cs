namespace VoltStream.WPF.Settings.ViewModels;

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
using VoltStream.WPF.Commons;
using VoltStream.WPF.Commons.ViewModels;

public partial class SettingsPageViewModel : ViewModelBase
{
    private readonly IMapper mapper;
    private readonly IServiceProvider services;

    public SettingsPageViewModel(IServiceProvider services)
    {
        this.services = services;
        mapper = services.GetRequiredService<IMapper>();
        apiConnection = services.GetRequiredService<ApiConnectionViewModel>();

        _ = LoadData();
    }

    [ObservableProperty] private ApiConnectionViewModel apiConnection;
    [ObservableProperty] private ObservableCollection<CurrencyViewModel> currencies = [];
    [ObservableProperty] private ObservableCollection<CategoryResponse> categories = [];
    [ObservableProperty] private ObservableCollection<ProductResponse> products = [];
    [ObservableProperty] private ObservableCollection<CustomerResponse> customers = [];

    [ObservableProperty] private CategoryResponse? selectedOldCategory;
    [ObservableProperty] private string newCategoryName = string.Empty;

    [ObservableProperty] private ProductResponse? selectedOldProduct;
    [ObservableProperty] private string newProductName = string.Empty;

    [ObservableProperty] private CustomerResponse? selectedOldCustomer;
    [ObservableProperty] private string newCustomerName = string.Empty;

    [ObservableProperty] private ProductResponse? selectedProductToChangeCategory;
    [ObservableProperty] private CategoryResponse? selectedNewCategory;
    [ObservableProperty] private string currentCategoryName = string.Empty;

    #region Commands

    [RelayCommand]
    private void AddCurrency()
    {
        Currencies.Add(new CurrencyViewModel());
    }

    [RelayCommand]
    private void RemoveCurrency(CurrencyViewModel currency)
    {
        Currencies.Remove(currency);
    }

    [RelayCommand]
    private async Task SaveCurrencies()
    {
        if (Currencies is null || Currencies.Count == 0)
        {
            Warning = "Saqlash uchun valyuta yo‘q";
            return;
        }

        var client = services.GetRequiredService<ICurrenciesApi>();
        var dtoList = mapper.Map<List<CurrencyRequest>>(Currencies);

        var response = await client.SaveAllAsync(dtoList)
            .Handle(isLoading => IsSelected = isLoading);

        if (response.IsSuccess) Success = "O'zgarishlar muvaffaqiyatli saqlandi";
        else Error = response.Message ?? "Valyutalarni saqlashda xatolik";
    }

    [RelayCommand]
    private async Task UpdateCategory()
    {
        if (SelectedOldCategory == null || string.IsNullOrWhiteSpace(NewCategoryName))
        {
            Warning = "Kategoriya tanlanmagan yoki yangi nom bo'sh";
            return;
        }

        var client = services.GetRequiredService<ICategoriesApi>();
        var response = await client.UpdateAsync(new() { Id = SelectedOldCategory.Id, Name = NewCategoryName })
            .Handle(isLoading => IsLoading = isLoading);

        if (response.IsSuccess)
        {
            Success = "Kategoriya nomi muvaffaqiyatli o'zgartirildi";
            _ = LoadCategories();
            NewCategoryName = string.Empty;
        }
        else Error = response.Message ?? "Kategoriyani yangilashda xatolik";
    }

    [RelayCommand]
    private async Task UpdateProduct()
    {
        if (SelectedOldProduct == null || string.IsNullOrWhiteSpace(NewProductName))
        {
            Warning = "Mahsulot tanlanmagan yoki yangi nom bo'sh";
            return;
        }

        var client = services.GetRequiredService<IProductsApi>();
        var response = await client.UpdateAsync(new ProductRequest
        {
            Id = SelectedOldProduct.Id,
            Name = NewProductName,
            Unit = SelectedOldProduct.Unit,
            CategoryId = SelectedOldProduct.CategoryId
        })
            .Handle(isLoading => IsLoading = isLoading);

        if (response.IsSuccess)
        {
            Success = "Mahsulot nomi muvaffaqiyatli o'zgartirildi";
            _ = LoadProducts();
            NewProductName = string.Empty;
        }
        else Error = response.Message ?? "Mahsulotni yangilashda xatolik";
    }

    [RelayCommand]
    private async Task UpdateCustomer()
    {
        if (SelectedOldCustomer == null || string.IsNullOrWhiteSpace(NewCustomerName))
        {
            Warning = "Mijoz tanlanmagan yoki yangi nom bo'sh";
            return;
        }

        var client = services.GetRequiredService<ICustomersApi>();
        var response = await client.UpdateAsync(new CustomerRequest
        {
            Id = SelectedOldCustomer.Id,
            Name = NewCustomerName,
            Phone = SelectedOldCustomer.Phone,
            Address = SelectedOldCustomer.Address,
            Description = SelectedOldCustomer.Description
        })
            .Handle(isLoading => IsLoading = isLoading);

        if (response.IsSuccess)
        {
            Success = "Mijoz nomi muvaffaqiyatli o'zgartirildi";
            _ = LoadCustomers();
            NewCustomerName = string.Empty;
        }
        else Error = response.Message ?? "Mijozni yangilashda xatolik";
    }

    [RelayCommand]
    private async Task ChangeProductCategory()
    {
        if (SelectedProductToChangeCategory == null || SelectedNewCategory == null)
        {
            Warning = "Mahsulot yoki yangi kategoriya tanlanmagan";
            return;
        }

        var client = services.GetRequiredService<IProductsApi>();
        var response = await client.UpdateAsync(new ProductRequest
        {
            Id = SelectedProductToChangeCategory.Id,
            Name = SelectedProductToChangeCategory.Name,
            Unit = SelectedProductToChangeCategory.Unit,
            CategoryId = SelectedNewCategory.Id
        })
            .Handle(isLoading => IsLoading = isLoading);

        if (response.IsSuccess)
        {
            Success = "Mahsulot kategoriyasi muvaffaqiyatli o'zgartirildi";
            _ = LoadProducts();
        }
        else Error = response.Message ?? "Kategoriyani almashtirishda xatolik";
    }

    partial void OnSelectedProductToChangeCategoryChanged(ProductResponse? value)
    {
        CurrentCategoryName = value?.Category?.Name ?? string.Empty;
    }

    #endregion Commands

    #region Load Data

    private async Task LoadData()
    {
        await Task.WhenAll(
            LoadCurrencies(),
            LoadCategories(),
            LoadProducts(),
            LoadCustomers()
        );
    }

    private async Task LoadCurrencies()
    {
        var client = services.GetRequiredService<ICurrenciesApi>();
        var response = await client.GetAllAsync().Handle(isLoading => IsLoading = isLoading);

        if (response.IsSuccess)
            Currencies = mapper.Map<ObservableCollection<CurrencyViewModel>>(response.Data);
        else Error = response.Message ?? "Valyutalarni yuklashda xatolik";
    }

    private async Task LoadCategories()
    {
        var client = services.GetRequiredService<ICategoriesApi>();
        var response = await client.GetAllAsync().Handle(isLoading => IsLoading = isLoading);

        if (response.IsSuccess)
            Categories = new ObservableCollection<CategoryResponse>(response.Data);
    }

    private async Task LoadProducts()
    {
        var client = services.GetRequiredService<IProductsApi>();
        var request = new FilteringRequest
        {
            Filters = new Dictionary<string, List<string>>
            {
                { "Category", ["include"] }
            },
            Page = 0,
            PageSize = 0
        };

        var response = await client.Filter(request).Handle(isLoading => IsLoading = isLoading);

        if (response.IsSuccess)
            Products = new ObservableCollection<ProductResponse>(response.Data);
    }

    private async Task LoadCustomers()
    {
        var client = services.GetRequiredService<ICustomersApi>();
        var response = await client.GetAllAsync().Handle(isLoading => IsLoading = isLoading);

        if (response.IsSuccess)
            Customers = new ObservableCollection<CustomerResponse>(response.Data);
    }

    #endregion Load Data
}
