using ApiServices.DTOs.Products;
using ApiServices.Interfaces;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows;
using VoltStream.WPF.Commons;
using VoltStream.WPF.Products.Models;

public partial class ProductViewModel : ViewModelBase
{
    private readonly IServiceProvider services;

    public ProductViewModel(IServiceProvider services)
    {
        this.services = services;
    }

    [ObservableProperty] private Category? selectedCategory;
    private ObservableCollection<Category> _categories = new();
    public ObservableCollection<Category> Categories
    {
        get => _categories;
        set => SetProperty(ref _categories, value);
    }

    [ObservableProperty] private Product? selectedProduct;
    [ObservableProperty] private ObservableCollection<Product> products = new();

    [ObservableProperty] private ObservableCollection<ProductItemViewModel> productItems = new();

    public async Task LoadUsersAsync()
    {
        try
        {
            var response = await services.GetRequiredService<ICategoriesApi>().GetAllAsync();
            if (response.IsSuccessful && response.Content.Data != null)
            {
                Categories = new ObservableCollection<Category>(response.Content.Data);
            }
            else
            {
                MessageBox.Show("Foydalanuvchilarni yuklashda xatolik.");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Server bilan aloqa yo'q: {ex.Message}");
        }
    }
}
