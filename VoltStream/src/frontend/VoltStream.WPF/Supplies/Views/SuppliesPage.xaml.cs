namespace VoltStream.WPF.Supplies.Views;

using ApiServices.Extensions;
using ApiServices.Interfaces;
using ApiServices.Models;
using ApiServices.Models.Requests;
using ApiServices.Models.Responses;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using VoltStream.WPF.Commons;
using VoltStream.WPF.Sales.ViewModels;

public partial class SuppliesPage : Page
{
    private readonly IProductsApi productsApi;
    private readonly ICategoriesApi categoriesApi;
    private readonly ISuppliesApi suppliesApi;
    private readonly IWarehouseStocksApi warehouseItemsApi;
    private List<CategoryResponse> _allCategories = [];
    private ICollectionView? categoriesView;

    public SuppliesPage(IServiceProvider services)
    {
        InitializeComponent();

        // API xizmatlarini DI orqali olish
        productsApi = services.GetRequiredService<IProductsApi>();
        categoriesApi = services.GetRequiredService<ICategoriesApi>();
        suppliesApi = services.GetRequiredService<ISuppliesApi>();
        warehouseItemsApi = services.GetRequiredService<IWarehouseStocksApi>();
        supplyDate.SelectedDate = DateTime.Now;
        supplyDate.dateTextBox.Focus();

        cbxCategory.PreviewLostKeyboardFocus += CbxCategory_PreviewLostKeyboardFocus;
        cbxProduct.PreviewLostKeyboardFocus += CbxProduct_PreviewLostKeyboardFocus;
        _ = LoadData();
    }

    private async Task LoadData()
    {
        _allCategories = await LoadCategoriesAsync();
        cbxCategory.ItemsSource = _allCategories;
    }

    private void CbxProduct_LostFocus(object sender, RoutedEventArgs e)
    {
        if ((cbxCategory.SelectedItem is null ||
                    cbxCategory.SelectedItem is not null) &&
                    string.IsNullOrWhiteSpace(cbxCategory.Text) &&
                    cbxProduct.SelectedItem is not null)
        {
            var categorytId = (cbxProduct.SelectedItem as ProductResponse)!.CategoryId;
            cbxCategory.SelectedItem = _allCategories.FirstOrDefault(a => a.Id == categorytId);
        }
    }

    private async void CbxProduct_GotFocus(object sender, RoutedEventArgs e)
    {
        cbxProduct.ItemsSource = null;
        cbxProduct.IsDropDownOpen = true;

        // Hozirgi tanlangan category ni olish
        var selectedCategory = cbxCategory.SelectedItem as CategoryResponse;

        // 1️⃣ Agar tanlangan category mavjud bo‘lsa
        if (selectedCategory is not null && _allCategories.FirstOrDefault(a =>
                a.Name.Equals(cbxCategory.Text.Trim(), StringComparison.OrdinalIgnoreCase)) is not null)
        {
            // Shu category ga oid productlarni yuklash
            var products = await productsApi.GetAllByCategoryIdAsync(selectedCategory.Id).Handle();

            cbxProduct.ItemsSource = products.Data ?? [];
            return;
        }

        // 2️⃣ Agar foydalanuvchi hali hech nima tanlamagan bo‘lsa
        if (cbxCategory.SelectedItem is null &&
            string.IsNullOrWhiteSpace(cbxCategory.Text))
        {
            // Barcha productlarni yuklash
            var allProducts = await productsApi.GetAllAsync().Handle();
            cbxProduct.ItemsSource = allProducts.Data ?? [];
            return;
        }

        // 2️⃣ Agar foydalanuvchi hali hech nima tanlamagan bo‘lsa 
        if (cbxCategory.SelectedItem is not null &&
            string.IsNullOrWhiteSpace(cbxCategory.Text))
        {
            // Barcha productlarni yuklash
            var allProducts = await productsApi.GetAllAsync().Handle();
            cbxProduct.ItemsSource = allProducts.Data ?? [];
            return;
        }

        // — ProductResponse combobox bo‘sh bo‘lib qoladi
        cbxProduct.ItemsSource = new List<ProductViewModel>();
    }

    private async Task LoadSuppliesAsync()
    {
        try
        {
            var text = supplyDate.dateTextBox.Text;

            // Frontda faqat format tekshiruvi
            if (!string.IsNullOrWhiteSpace(text) &&
                DateTime.TryParse(text, out _)) // Tekshiradi, backend parsing qiladi
            {
                var filter = new FilteringRequest
                {
                    Filters = new()
                    {
                        ["date"] = [text],        // faqat string
                        ["product"] = ["include:category"]
                    },
                    Descending = true
                };

                var response = await suppliesApi.Filter(filter).Handle();
                if (response.IsSuccess)
                {
                    supplyDataGrid.ItemsSource = response.Data;
                }
                else
                {
                    MessageBox.Show($"Ta'minotlarni olishda xatolik: {response.Message ?? "Ma'lumotlar yo'q"}",
                        "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Iltimos, to‘g‘ri sana kiriting.", "Xato",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Server bilan ulanishda xatolik: {ex.Message}", "Xatolik",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async void TbxRollCount_GotFocus(object sender, RoutedEventArgs e)
    {
        try
        {
            // tbxPerRollCount va tbxRollCount qiymatlarini olish
            if (decimal.TryParse(tbxPerRollCount.Text, out decimal perRollCount) &&
                decimal.TryParse(tbxRollCount.Text, out decimal rollCount))
            {
                var warehouseItem = await warehouseItemsApi.GetAllWarehouseItemsAsync().Handle();
                // Ko‘paytma hisoblash
                decimal total = perRollCount * rollCount;
                // totalMeters ga joylash
                totalMeters.Text = total.ToString("N2");
            }
            else
            {
                // Agar qiymatlar noto‘g‘ri bo‘lsa, totalMeters ni bo‘sh qoldiramiz
                totalMeters.Text = string.Empty;
            }

            if (cbxProduct.SelectedValue is not null && long.TryParse(cbxProduct.SelectedValue.ToString(), out long productId))
            {


                var warehouseItems = await warehouseItemsApi.GetAllWarehouseItemsAsync().Handle();
                if (warehouseItems?.Data is not null)
                {
                    var warehouseItem = warehouseItems.Data.FirstOrDefault(x => x.ProductId == productId);
                    if (warehouseItem is not null)
                    {
                        txtPrice.Text = warehouseItem.UnitPrice.ToString("N2");
                        tbxDiscountPercent.Text = warehouseItem.DiscountRate.ToString("N2");
                    }
                }
            }

        }
        catch (Exception ex)
        {
            // Xatolik yuzaga kelsa, log yozish yoki xabar ko‘rsatish
            System.Diagnostics.Debug.WriteLine($"Xato: {ex.Message}");
            totalMeters.Text = string.Empty;
        }
    }

    private async void AddSupplyBtn_Click(object sender, RoutedEventArgs e)
    {
        addSupplyBtn.IsEnabled = false;
        try
        {
            if (supplyDate.SelectedDate is null)
            {
                MessageBox.Show("Sana tanlanmagan!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                supplyDate.Focus();
                return;
            }
            else
            {
                if (DateTime.TryParse(supplyDate.dateTextBox.Text, out DateTime parsedDate))
                {
                    supplyDate.SelectedDate = parsedDate;
                }
            }

            if (!decimal.TryParse(tbxPerRollCount.Text, out decimal perRollCount) || perRollCount <= 0)
            {
                MessageBox.Show("Rulon metr noto‘g‘ri kiritilgan!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                tbxPerRollCount.Focus();
                return;
            }

            if (!decimal.TryParse(tbxRollCount.Text, out decimal rollCount) || rollCount <= 0)
            {
                MessageBox.Show("Rulon soni noto‘g‘ri kiritilgan!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                tbxRollCount.Focus();
                return;
            }

            if (!decimal.TryParse(txtPrice.Text, out decimal price) || price < 0)
            {
                MessageBox.Show("Narx noto‘g‘ri kiritilgan!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                txtPrice.Focus();
                return;
            }

            if (!decimal.TryParse(tbxDiscountPercent.Text, out decimal discountPercent) || discountPercent < 0)
            {
                MessageBox.Show("Chegirma foizi noto‘g‘ri kiritilgan!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                tbxDiscountPercent.Focus();
                return;
            }

            decimal totalQuantity = perRollCount * rollCount;
            long categoryId = cbxCategory.SelectedValue is not null ? Convert.ToInt64(cbxCategory.SelectedValue) : 0;
            long productId = cbxProduct.SelectedValue is not null ? Convert.ToInt64(cbxProduct.SelectedValue) : 0;

            var supply = new SupplyRequest
            {
                Date = supplyDate.SelectedDate.Value.ToUniversalTime(),
                CategoryId = categoryId,
                ProductId = productId,
                RollCount = rollCount,
                LengthPerRoll = perRollCount,
                TotalLength = totalQuantity,
                ProductName = ((ProductResponse)cbxProduct.SelectedItem)?.Name ?? cbxProduct.Text ?? string.Empty,
                CategoryName = ((CategoryResponse)cbxCategory.SelectedItem)?.Name ?? cbxCategory.Text ?? string.Empty,
                UnitPrice = price,
                DiscountRate = discountPercent
            };

            var response = await suppliesApi.CreateSupplyAsync(supply).Handle();

            if (response.IsSuccess)
            {
                // Formani tozalash
                cbxCategory.SelectedItem = null;
                cbxCategory.Text = null;
                cbxProduct.SelectedItem = null;
                cbxProduct.Text = null;
                tbxPerRollCount.Clear();
                tbxRollCount.Clear();
                totalMeters.Clear();
                txtPrice.Clear();
                tbxDiscountPercent.Clear();

                await LoadSuppliesAsync();

            }
            else
            {
                MessageBox.Show($"Ta'minot qo‘shishda xatolik: {response.Message ?? "Ma'lumotlar yo‘q"}",
                    "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Server bilan ulanishda xatolik: {ex.Message}",
                "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            addSupplyBtn.IsEnabled = true;
            cbxCategory.Focus();
        }
    }

    private void TxtPrice_GotFocus(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox tb)
        {
            tb.Dispatcher.BeginInvoke(new Action(tb.SelectAll));
        }
    }

    private void TxtPrice_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is TextBox tb && !tb.IsKeyboardFocusWithin)
        {
            e.Handled = true;
            tb.Focus();
        }
    }

    private void TbxDiscountPercent_GotFocus(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox tb)
        {
            tb.Dispatcher.BeginInvoke(new Action(tb.SelectAll));
        }
    }

    private void TbxDiscountPercent_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is TextBox tb && !tb.IsKeyboardFocusWithin)
        {
            e.Handled = true;
            tb.Focus();
        }
    }

    private async void SupplyDate_LostFocus(object sender, RoutedEventArgs e)
    {
        await LoadSuppliesAsync();
    }

    private async void SupplyDate_GotFocus(object sender, RoutedEventArgs e)
    {
        await LoadSuppliesAsync();
    }

    //private void SupplyDate_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    //{
    //    if (sender is VoltStream.WPF.Commons.UserControls.UserCalendar uc)
    //    {
    //        // Ichidagi dateTextBox’ga fokus berish
    //        if (uc.FindName("dateTextBox") is TextBox textBox && !textBox.IsKeyboardFocusWithin)
    //        {
    //            e.Handled = true;
    //            textBox.Focus();
    //            textBox.SelectAll();   // Ixtiyoriy: matnni belgilang
    //        }
    //    }
    //}

    private async Task<List<CategoryResponse>> LoadCategoriesAsync()
    {
        try
        {
            var response = await categoriesApi.GetAllAsync().Handle();
            if (response.IsSuccess)
            {
                return response.Data!;
            }
            MessageBox.Show("Kategoriyalar topilmadi.", "Ma'lumot",
                            MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Server bilan ulanishda xatolik: {ex.Message}",
                            "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        return [];
    }

    private async void CbxCategory_GotFocus(object sender, RoutedEventArgs e)
    {
        _allCategories = await LoadCategoriesAsync();

        categoriesView = CollectionViewSource.GetDefaultView(_allCategories);
        cbxCategory.ItemsSource = categoriesView;

        cbxCategory.IsDropDownOpen = true;
    }
    private void CbxCategory_LostFocus(object sender, RoutedEventArgs e)
    {
        cbxCategory.IsDropDownOpen = false;

    }

    private void CbxCategory_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        try
        {
            // Text null emasligini tekshir
            var text = cbxCategory.Text?.Trim();
            if (string.IsNullOrEmpty(text))
                return;

            // Kategoriya ro‘yxati mavjudmi?
            if (_allCategories is null || _allCategories.Count == 0)
            {
                ComboBoxHelper.BeforeUpdate(sender, e, "Mahsulot turi", true);
                return;
            }

            // Ro‘yxatda shunday nom yo‘qligini tekshir
            bool exists = _allCategories
                .Any(a => !string.IsNullOrEmpty(a?.Name) &&
                          a.Name.Equals(text, StringComparison.OrdinalIgnoreCase));

            if (!exists)
            {
                ComboBoxHelper.BeforeUpdate(sender, e, "Mahsulot turi", true);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void CbxProduct_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        ComboBoxHelper.BeforeUpdate(sender, e, "Mahsulot nomi", true);
    }
}
