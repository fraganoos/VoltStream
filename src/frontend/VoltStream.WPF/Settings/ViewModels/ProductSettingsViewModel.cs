namespace VoltStream.WPF.Settings.ViewModels;

using ApiServices.Extensions;
using ApiServices.Interfaces;
using ApiServices.Models;
using ApiServices.Models.Requests;
using ApiServices.Models.Responses;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows;
using VoltStream.WPF.Commons;

public partial class ProductSettingsViewModel : ViewModelBase
{
    private readonly IProductsApi productsApi;
    private readonly ICategoriesApi categoriesApi;

    public ProductSettingsViewModel(IServiceProvider services)
    {
        productsApi = services.GetRequiredService<IProductsApi>();
        categoriesApi = services.GetRequiredService<ICategoriesApi>();
        _ = LoadData();
    }

    [ObservableProperty] private ObservableCollection<ProductResponse> products = [];
    [ObservableProperty] private ObservableCollection<CategoryResponse> categories = [];
    [ObservableProperty] private ProductResponse? selectedProduct;
    [ObservableProperty] private CategoryResponse? selectedCategory;
    [ObservableProperty] private string name = string.Empty;
    [ObservableProperty] private string unit = string.Empty;
    [ObservableProperty] private bool isEditing;

    public async Task LoadData()
    {
        await Task.WhenAll(LoadProducts(), LoadCategories());
    }

    private async Task LoadProducts()
    {
        FilteringRequest request = new()
        {
            Filters = new()
            {
                ["category"] = ["include"]
            },
            SortBy = "name"
        };

        var response = await productsApi.Filter(request).Handle(isLoading => IsLoading = isLoading);
        if (response.IsSuccess)
            Products = new ObservableCollection<ProductResponse>(response.Data);
        else
            Error = response.Message ?? "Mahsulotlarni yuklashda xatolik!";
    }

    private async Task LoadCategories()
    {
        var response = await categoriesApi.GetAllAsync().Handle();
        if (response.IsSuccess)
            Categories = new ObservableCollection<CategoryResponse>(response.Data);
    }

    [RelayCommand]
    private async Task Save()
    {
        if (string.IsNullOrWhiteSpace(Name) || SelectedCategory == null) return;

        if (IsEditing && SelectedProduct != null)
        {
            var request = new ProductRequest
            {
                Id = SelectedProduct.Id,
                Name = Name,
                Unit = Unit,
                CategoryId = SelectedCategory.Id
            };

            var response = await productsApi.UpdateAsync(request).Handle(isLoading => IsLoading = isLoading);

            if (response.IsSuccess)
            {
                Success = "Muvaffaqiyatli saqlandi!";
                await LoadProducts();
                Cancel();
            }
            else
            {
                Error = response.Message ?? "Mahsulotni ma'lumotlarini yangilashda xatolik!";
            }
        }
        else
        {
            var request = new ProductRequest
            {
                Name = Name,
                Unit = Unit,
                CategoryId = SelectedCategory.Id
            };

            var response = await productsApi.CreateAsync(request).Handle(isLoading => IsLoading = isLoading);

            if (response.IsSuccess)
            {
                Success = "Muvaffaqiyatli yaratildi";
                await LoadProducts();
                Cancel();
            }
            else
            {
                Error = response.Message ?? "Mahsulot yaratishda xatolik";
            }
        }
    }

    [RelayCommand]
    private void Edit(ProductResponse product)
    {
        SelectedProduct = product;
        Name = product.Name;
        Unit = product.Unit;
        SelectedCategory = Categories.FirstOrDefault(c => c.Id == product.CategoryId);
        IsEditing = true;
    }

    [RelayCommand]
    private async Task Delete(ProductResponse product)
    {
        if (MessageBox.Show($"{product.Name}  mahsulotni o'chirishni tasdiqlaysizmi?", "O'chirish", MessageBoxButton.YesNo) != MessageBoxResult.Yes) return;

        var response = await productsApi.DeleteAsync(product.Id).Handle(isLoading => IsLoading = isLoading);

        if (response.IsSuccess)
        {
            Success = "Muvaffaqiyatli o'chirildi!";
            await LoadProducts();
            if (SelectedProduct == product) Cancel();
        }
        else
        {
            Error = response.Message ?? "Mahsulotni o'chirishda xatolik!";
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        SelectedProduct = null;
        SelectedCategory = null;
        Name = string.Empty;
        Unit = string.Empty;
        IsEditing = false;
    }
}
