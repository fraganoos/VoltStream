namespace VoltStream.WPF.Sales.Views;

using ApiServices.DTOs.Customers;
using ApiServices.DTOs.Products;
using ApiServices.DTOs.Supplies;
using ApiServices.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VoltStream.WPF.Commons;
using VoltStream.WPF.Sales.Models;

/// <summary>
/// Логика взаимодействия для SalesPage.xaml
/// </summary>
public partial class SalesPage : Page
{
    private readonly IServiceProvider services;
    private readonly ICategoriesApi categoriesApi;
    private readonly IProductsApi productsApi;
    private readonly IWarehouseItemsApi warehouseItemsApi;
    private readonly ICustomersApi customersApi;

    public Sale sale = new();

    public SalesPage(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
        DataContext = sale;
        categoriesApi = services.GetRequiredService<ICategoriesApi>();
        productsApi = services.GetRequiredService<IProductsApi>();
        warehouseItemsApi = services.GetRequiredService<IWarehouseItemsApi>();
        customersApi = services.GetRequiredService<ICustomersApi>();

        CustomerName.GotFocus += CustomerName_GotFocus;
        cbxCategoryName.GotFocus += CbxCategoryName_GotFocus;
        cbxCategoryName.PreviewLostKeyboardFocus += CbxCategoryName_PreviewLostKeyboardFocus;

        cbxProductName.GotFocus += CbxProductName_GotFocus;
        cbxProductName.SelectionChanged += CbxProductName_SelectionChanged;
        cbxProductName.PreviewLostKeyboardFocus += CbxProductName_PreviewLostKeyboardFocus;
        cbxProductName.LostFocus += CbxProductName_LostFocus;

        cbxPerRollCount.GotFocus += CbxPerRollCount_GotFocus;
        cbxPerRollCount.SelectionChanged += CbxPerRollCount_SelectionChanged;
        cbxPerRollCount.PreviewLostKeyboardFocus += CbxPerRollCount_PreviewLostKeyboardFocus;
    }

    private void CbxPerRollCount_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (cbxPerRollCount.SelectedItem is WarehouseItem selectedWarehouseItem)
        {
            // Rulon tanlanganda, uning narxi, chegirmasi o'zgartiramiz
            tbxPrice.Text = selectedWarehouseItem.Price.ToString();
            tbxPerDiscount.Text = selectedWarehouseItem.DiscountPercent.ToString();
        }
    }

    private async void CustomerName_GotFocus(object sender, RoutedEventArgs e) /// API Qilish kerak
    {
        await LoadCustomerNameAsync();
    }

    private async Task LoadCustomerNameAsync()
    {
        try
        {
            // Сохраняем текущее выбранное значение
            var selectedValue = CustomerName.SelectedValue;
            var response = await customersApi.GetAllCustomersAsync();

                if (response.IsSuccessStatusCode && response.Content?.Data != null)
                {
                    List<Customer> customers = response.Content.Data;
                    CustomerName.ItemsSource = customers;
                    CustomerName.DisplayMemberPath = "Name";
                    CustomerName.SelectedValuePath = "Id";
                    // Восстанавливаем выбранное значение
                    if (selectedValue != null)
                        CustomerName.SelectedValue = selectedValue;
                }
                else
                {
                    // Проверяем на null, чтобы избежать CS8602
                    var errorMsg = response.Error?.Message ?? "Unknown error";
                    MessageBox.Show("Error fetching customers: " + errorMsg);
                }

        }
        catch (Exception ex)
        {
            MessageBox.Show("An error occurred: " + ex.Message);
        }
    }

    private async void CbxCategoryName_GotFocus(object sender, RoutedEventArgs e)
    {
        await LoadCategoryAsync();
        cbxCategoryName.IsDropDownOpen = true;
    }
    private void CbxCategoryName_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        ComboBoxHelper.BeforeUpdate(sender, e, "Maxsulot turi");
    }

    private async void CbxProductName_GotFocus(object sender, RoutedEventArgs e)
    {
        long? categoryId = null;
        if (cbxCategoryName.SelectedValue != null)
        {
            categoryId = (long)cbxCategoryName.SelectedValue;
        }
        await LoadProductAsync(categoryId);
        cbxProductName.IsDropDownOpen = true;
    }
    private void CbxProductName_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (cbxProductName.SelectedItem is Product selectedProduct)
        {
            // maxsulot tanlanganda, uning categoryId sini ham olamiz va cbxCategoryName dagini o'zgartiramiz
            cbxCategoryName.SelectedValue = selectedProduct.CategoryId;
            //sale.CategoryId = selectedProduct.CategoryId;
            //sale.CategoryName = (cbxCategoryName.ItemsSource as IEnumerable<Category>)?
            //    .FirstOrDefault(c => c.Id == selectedProduct.CategoryId)?.Name ?? string.Empty;
        }
    }
    private void CbxProductName_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        ComboBoxHelper.BeforeUpdate(sender, e, "Maxsulot");
    }
    private async void CbxProductName_LostFocus(object sender, RoutedEventArgs e)
    {
        long? productId = null;
        if (cbxProductName.SelectedValue != null)
        {
            productId = (long)cbxProductName.SelectedValue;
        }
        await LoadWarehouseItemsAsync(productId);
    }

    private void CbxPerRollCount_GotFocus(object sender, RoutedEventArgs e)
    {
        cbxPerRollCount.IsDropDownOpen = true;
    }
    private void CbxPerRollCount_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        ComboBoxHelper.BeforeUpdate(sender, e, "Rulon uzunlugi");
    }


    private async Task LoadCategoryAsync() // Загрузка категорий
    {
        try
        {
            // Сохраняем текущее выбранное значение
            var selectedValue = cbxCategoryName.SelectedValue;

            var response = await categoriesApi.GetAllCategoriesAsync();
            if (response.IsSuccessStatusCode && response.Content?.Data != null)
            {
                List<Category> categories = response.Content.Data;
                cbxCategoryName.ItemsSource = categories;
                cbxCategoryName.DisplayMemberPath = "Name";
                cbxCategoryName.SelectedValuePath = "Id";

                // Восстанавливаем выбранное значение
                if (selectedValue != null)
                    cbxCategoryName.SelectedValue = selectedValue;
            }
            else
            {
                // Проверяем на null, чтобы избежать CS8602
                var errorMsg = response.Error?.Message ?? "Unknown error";
                MessageBox.Show("Error fetching categories: " + errorMsg);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("An error occurred: " + ex.Message);
        }
    }

    private async Task LoadProductAsync(long? categoryId) // Загрузка продукции
    {
        try
        {
            // Сохраняем текущее выбранное значение
            var selectedValue = cbxProductName.SelectedValue;
            ApiResponse<Response<List<Product>>> response;

            if (categoryId.HasValue && categoryId.Value != 0)
            {
                response = await productsApi.GetAllProductsByCategoryIdAsync(categoryId.Value);
            }
            else
            {
                response = await productsApi.GetAllProductsAsync();
            }

            if (response.IsSuccessStatusCode && response.Content?.Data != null)
            {
                var products = response.Content.Data;
                cbxProductName.ItemsSource = products;
                cbxProductName.DisplayMemberPath = "Name";
                cbxProductName.SelectedValuePath = "Id";
                // Восстанавливаем выбранное значение
                if (selectedValue != null)
                    cbxProductName.SelectedValue = selectedValue;
            }
            else
            {
                var errorMsg = response.Error?.Message ?? "Unknown error";
                MessageBox.Show("Ошибка при получении продукции: " + errorMsg);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Произошла ошибка: " + ex.Message);
        }
    }

    private async Task LoadWarehouseItemsAsync(long? productId) // Загрузка данных со склада по productId
    {
        try
        {
            // Сохраняем текущее выбранное значение
            var selectedValue = cbxPerRollCount.SelectedValue;
            ApiResponse<Response<List<WarehouseItem>>> response;
            if (productId.HasValue && productId.Value != 0)
            {
                response = await warehouseItemsApi.GetProductDetailsFromWarehouseAsync(productId.Value);
            }
            else
            {
                // Если productId не задан, можно либо не загружать данные, либо загрузить все элементы склада
                // Здесь я выбрал не загружать ничего
                cbxPerRollCount.ItemsSource = null;
                return;
            }
            if (response.IsSuccessStatusCode && response.Content?.Data != null)
            {
                var warehouseItems = response.Content.Data;
                cbxPerRollCount.ItemsSource = warehouseItems;
                cbxPerRollCount.DisplayMemberPath = "QuantityPerRoll";
                cbxPerRollCount.SelectedValuePath = "QuantityPerRoll";
                // Восстанавливаем выбранное значение
                if (selectedValue != null)
                    cbxPerRollCount.SelectedValue = selectedValue;
            }
            else
            {
                var errorMsg = response.Error?.Message ?? "Unknown error";
                MessageBox.Show("Ошибка при получении данных со склада: " + errorMsg);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Произошла ошибка: " + ex.Message);
        }
    }
}

