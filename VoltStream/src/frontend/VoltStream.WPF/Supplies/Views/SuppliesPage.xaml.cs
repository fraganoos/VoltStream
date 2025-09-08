namespace VoltStream.WPF.Supplies.Views;


using ApiServices.DTOs.Products;
using ApiServices.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

public partial class SuppliesPage : Page
{
    private readonly IServiceProvider services;
    private readonly IProductsApi productsApi;
    private readonly ICategoriesApi categoriesApi;

    public SuppliesPage(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;

        // API xizmatlarini DI orqali olish
        productsApi = services.GetRequiredService<IProductsApi>();
        categoriesApi = services.GetRequiredService<ICategoriesApi>();

        // Sahifa yuklanganda kategoriyalarni chaqirish
        this.Loaded += SuppliesPage_Loaded;
    }

    private async void SuppliesPage_Loaded(object sender, RoutedEventArgs e)
    {
        await LoadCategoriesAsync();
        await LoadProductsAsync();
    }

    private async Task LoadCategoriesAsync()
    {
        try
        {
            var response = await categoriesApi.GetAllCategoriesAsync();
            if (response.IsSuccessStatusCode && response.Content?.Data != null)
            {
                List<Category> categories = response.Content.Data;
                cbxCategory.ItemsSource = categories;
                cbxCategory.DisplayMemberPath = "Name";
                cbxCategory.SelectedValuePath = "Id";
                if (categories.Count == 0)
                    MessageBox.Show("Kategoriyalar topilmadi.", "Ma'lumot", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"Kategoriyalarni olishda xatolik: {response.Error?.Message ?? "Ma'lumotlar yo'q"}",
                    "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Server bilan ulanishda xatolik: {ex.Message}", "Xatolik",
                MessageBoxButton.OK, MessageBoxImage.Error);
            System.Diagnostics.Debug.WriteLine($"Xato: {ex.StackTrace}");
        }
    }
    private async Task LoadProductsAsync()
    {
        try
        {
            var response = await productsApi.GetAllProductsAsync();
            if (response.IsSuccessStatusCode && response.Content?.Data != null)
            {
                List<Product> products = response.Content.Data;
                cbxProduct.ItemsSource = products;
                cbxProduct.DisplayMemberPath = "Name";
                cbxProduct.SelectedValuePath = "Id";
                if (products.Count == 0)
                    MessageBox.Show("Mahsulotlar topilmadi.", "Ma'lumot", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show($"Mahsulotlar olishda xatolik: {response.Error?.Message ?? "Ma'lumotlar yo'q"}",
                    "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Server bilan ulanishda xatolik: {ex.Message}", "Xatolik",
                MessageBoxButton.OK, MessageBoxImage.Error);
            System.Diagnostics.Debug.WriteLine($"Xato: {ex.StackTrace}");
        }
    }

    private void addSupplyBtn_Click(object sender, RoutedEventArgs e)
    {
        // Bu yerda keyinchalik qo‘shish funksiyasi yoziladi
    }
}
