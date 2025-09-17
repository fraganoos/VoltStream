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

namespace VoltStream.WPF.Sales.Views
{
    /// <summary>
    /// Логика взаимодействия для SalesPage.xaml
    /// </summary>
    public partial class SalesPage : Page
    {
        private readonly IServiceProvider services;
        private readonly ICategoriesApi categoriesApi;
        private readonly IProductsApi productsApi;
        private readonly IWarehouseItemsApi warehouseItemsApi;

        public Sale _sale = new Sale();

        public SalesPage(IServiceProvider services)
        {
            InitializeComponent();
            this.services = services;
            DataContext = _sale;
            categoriesApi = services.GetRequiredService<ICategoriesApi>();
            productsApi = services.GetRequiredService<IProductsApi>();
            warehouseItemsApi = services.GetRequiredService<IWarehouseItemsApi>();

            cbxProductName.PreviewLostKeyboardFocus += cbxProductName_PreviewLostKeyboardFocus;
            cbxCategoryName.PreviewLostKeyboardFocus += CbxCategoryName_PreviewLostKeyboardFocus;
            cbxPerRollCount.PreviewLostKeyboardFocus += CbxPerRollCount_PreviewLostKeyboardFocus;
            cbxProductName.SelectionChanged += CbxProductName_SelectionChanged;
        }

        private void CbxProductName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbxProductName.SelectedItem is Product selectedProduct)
            {
                // maxsulot tanlanganda, uning categoryId sini ham olamiz va cbxCategoryName dagini o'zgartiramiz
                cbxCategoryName.SelectedValue = selectedProduct.CategoryId;
                //_sale.CategoryId = selectedProduct.CategoryId;
                //_sale.CategoryName = (cbxCategoryName.ItemsSource as IEnumerable<Category>)?
                //    .FirstOrDefault(c => c.Id == selectedProduct.CategoryId)?.Name ?? string.Empty;
            }
        }

        private void CbxPerRollCount_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ComboBoxHelper.BeforeUpdate(sender, e, "Rulon uzunlugi", true);
        }

        private void CbxCategoryName_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ComboBoxHelper.BeforeUpdate(sender, e, "Maxsulot turi");
        }

        private void cbxProductName_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ComboBoxHelper.BeforeUpdate(sender, e, "Maxsulot");
        }

        private async void cbxCategoryName_GotFocus(object sender, RoutedEventArgs e)
        {
            await LoadCategoryAsync();
            cbxCategoryName.IsDropDownOpen = true;

        }

        private async void cbxPoductName_GotFocus(object sender, RoutedEventArgs e)
        {
            long? categoryId = null;
            if (cbxCategoryName.SelectedValue != null)
            {
                categoryId = (long)cbxCategoryName.SelectedValue;
            }
            await LoadProductAsync(categoryId);
            cbxProductName.IsDropDownOpen = true;
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

    }
}
