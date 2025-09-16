using ApiServices.DTOs.Products;
using ApiServices.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
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

        public Sale _sale = new Sale();

        public SalesPage(IServiceProvider services)
        {
            InitializeComponent();
            this.services = services;
            DataContext = _sale;
            categoriesApi = services.GetRequiredService<ICategoriesApi>();
        }

        // Пошаговый план (псевдокод):
        // 1. Проверить, не сбрасывается ли ItemsSource или SelectedValue при каждом фокусе.
        // 2. При загрузке категорий сохранять выбранное значение и восстанавливать его после обновления ItemsSource.
        // 3. Не очищать ItemsSource, если данные не изменились.

        private async Task LoadCategoryAsync()
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
        private async void cbxCategoryName_GotFocus(object sender, RoutedEventArgs e)
        {
            await LoadCategoryAsync();

        }
    }
}
