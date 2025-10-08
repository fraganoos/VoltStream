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
        LoadWarehouseItemsAsynce();
    }

    [ObservableProperty] private Category? selectedCategory;
    [ObservableProperty] private ObservableCollection<Category> categories = new();

    [ObservableProperty] private Product? selectedProduct;
    [ObservableProperty] private ObservableCollection<Product> products = new();
 
    [ObservableProperty] private ObservableCollection<ProductItemViewModel> productItems = new();


    public async Task LoadWarehouseItemsAsynce()
    {
        try
        {
            var response = await services.GetRequiredService<IWarehouseItemsApi>()
                .GetAllWarehouseItemsAsync();
            if (response.IsSuccessful && response.Content.Data != null)
            {
                ProductItems.Clear();
                foreach (var item in response.Content.Data)
                {
                    ProductItems.Add(new ProductItemViewModel
                    {
                        Category = item.Product.Category.Name,
                        Name = item.Product.Name,
                        RollLength = item.QuantityPerRoll,
                        Quantity = item.CountRoll,
                        Price = item.Price,
                        TotalCount = (int)item.TotalQuantity,
                    });
                }
            }
        }
        catch (Exception ex) 
        { }
    }
    public async Task LoadCategoriesAsync()
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
    public async Task LoadProductsAsync()
    {
        try
        {
            var response = await services.GetRequiredService<IProductsApi>().GetAllAsync();
            if (response.IsSuccessful && response.Content.Data != null)
            {
                Products = new ObservableCollection<Product>(response.Content.Data);
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
