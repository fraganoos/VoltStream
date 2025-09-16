using ApiServices.DTOs.Products;
using ApiServices.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Refit;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        public Sale _sale = new Sale();

        public SalesPage(IServiceProvider services)
        {
            InitializeComponent();
            this.services = services;
            DataContext = _sale;
            categoriesApi = services.GetRequiredService<ICategoriesApi>();
            productsApi = services.GetRequiredService<IProductsApi>();
            cbxProductName.PreviewLostKeyboardFocus += cbxProductName_PreviewLostKeyboardFocus;
            cbxCategoryName.PreviewLostKeyboardFocus += CbxCategoryName_PreviewLostKeyboardFocus;

        }

        private void CbxCategoryName_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            ComboBox_PreviewLostKeyboardFocus(sender, e, "Maxsulot turi");
        }

        private void cbxProductName_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox == null) return;
            var inputText = comboBox.Text?.Trim();
            if (string.IsNullOrEmpty(inputText)) return;

            var products = comboBox.ItemsSource as IEnumerable<Product>;
            if (products == null || !products.Any())
            {
                e.Handled = true; // отменяем потерю фокуса
                MessageBox.Show("Список продукции пуст.");
                return;
            }

            bool found = products.Any(p => string.Equals(p.Name, inputText, StringComparison.OrdinalIgnoreCase));
            if (!found)
            {
                e.Handled = true; // отменяем потерю фокуса
                MessageBox.Show("Продукция не найдена в списке");
            }
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
        
        
        private void ComboBox_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e, string? strInfo) // ComboBox лар учун Универсал обработчик для, ItemsSource билан солиштиради
        {
            var comboBox = sender as ComboBox;
            if (comboBox == null) return;
            var inputText = comboBox.Text?.Trim();
            if (string.IsNullOrEmpty(inputText)) return;

            // Определяем тип элементов в ItemsSource
            var items = comboBox.ItemsSource;
            if (items == null)
            {
                e.Handled = true;
                MessageBox.Show($"{strInfo} - Ro'yxati bo'sh.","Tekshiruv");
                return;
            }

            // Получаем свойство для сравнения (DisplayMemberPath)
            var displayMember = comboBox.DisplayMemberPath;
            bool found = false;

            foreach (var item in items)
            {
                string value = item?.ToString() ?? "";
                if (!string.IsNullOrEmpty(displayMember))
                {
                    var prop = item.GetType().GetProperty(displayMember);
                    if (prop != null)
                    {
                        value = prop.GetValue(item)?.ToString() ?? "";
                    }
                }
                if (string.Equals(value, inputText, StringComparison.OrdinalIgnoreCase))
                {
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                e.Handled = true;
                MessageBox.Show($"{inputText} - {strInfo}: ro'yxatda yo'q.", "Tekshiruv");
            }
        }
        
    }
}
