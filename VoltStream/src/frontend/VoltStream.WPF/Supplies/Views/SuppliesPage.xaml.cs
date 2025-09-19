namespace VoltStream.WPF.Supplies.Views;

using ApiServices.DTOs.Products;
using ApiServices.DTOs.Supplies;
using ApiServices.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using VoltStream.WPF.Commons;

public partial class SuppliesPage : Page
{
    private readonly IProductsApi productsApi;
    private readonly ICategoriesApi categoriesApi;
    private readonly ISuppliesApi suppliesApi;
    private readonly IWarehouseItemsApi warehouseItemsApi;
    private List<Category> _allCategories = [];
    private ICollectionView categoriesView;

    public SuppliesPage(IServiceProvider services)
    {
        InitializeComponent();

        // API xizmatlarini DI orqali olish
        productsApi = services.GetRequiredService<IProductsApi>();
        categoriesApi = services.GetRequiredService<ICategoriesApi>();
        suppliesApi = services.GetRequiredService<ISuppliesApi>();
        warehouseItemsApi = services.GetRequiredService<IWarehouseItemsApi>();
        supplyDate.SelectedDate = DateTime.Now;
        supplyDate.dateTextBox.Focus();

        cbxCategory.PreviewLostKeyboardFocus += cbxCategory_PreviewLostKeyboardFocus;
        cbxProduct.PreviewLostKeyboardFocus += cbxProduct_PreviewLostKeyboardFocus;
    }

    private void cbxProduct_LostFocus(object sender, RoutedEventArgs e)
    {

        if ((cbxCategory.SelectedItem is null ||
                    cbxCategory.SelectedItem is not null) &&
                    string.IsNullOrWhiteSpace(cbxCategory.Text) &&
                    cbxProduct.SelectedItem is not null)
        {
            var categorytId = (cbxProduct.SelectedItem as Product)!.CategoryId;
            cbxCategory.SelectedItem = _allCategories.FirstOrDefault(a => a.Id == categorytId);
        }
    }

    private async void cbxProduct_GotFocus(object sender, RoutedEventArgs e)
    {
        cbxProduct.ItemsSource = null;
        cbxProduct.IsDropDownOpen = true;

        // Hozirgi tanlangan category ni olish
        var selectedCategory = cbxCategory.SelectedItem as Category;

        // 1️⃣ Agar tanlangan category mavjud bo‘lsa
        if (selectedCategory is not null && _allCategories.FirstOrDefault(a =>
                a.Name.Equals(cbxCategory.Text.Trim(), StringComparison.OrdinalIgnoreCase)) is not null)
        {
            // Shu category ga oid productlarni yuklash
            var products = await productsApi.GetAllProductsByCategoryIdAsync(selectedCategory.Id);

            cbxProduct.ItemsSource = products.Content!.Data ?? [];
            return;
        }

        // 2️⃣ Agar foydalanuvchi hali hech nima tanlamagan bo‘lsa
        if (cbxCategory.SelectedItem is null &&
            string.IsNullOrWhiteSpace(cbxCategory.Text))
        {
            // Barcha productlarni yuklash
            var allProducts = await productsApi.GetAllProductsAsync();
            cbxProduct.ItemsSource = allProducts.Content!.Data ?? [];
            return;
        }

        // 2️⃣ Agar foydalanuvchi hali hech nima tanlamagan bo‘lsa 
        if (cbxCategory.SelectedItem is not null &&
            string.IsNullOrWhiteSpace(cbxCategory.Text))
        {
            // Barcha productlarni yuklash
            var allProducts = await productsApi.GetAllProductsAsync();
            cbxProduct.ItemsSource = allProducts.Content!.Data ?? [];
            return;
        }

        // — Product combobox bo‘sh bo‘lib qoladi
        cbxProduct.ItemsSource = new List<Product>();
    }

    private async Task LoadSuppliesAsync()
    {
        try
        {
            var text = supplyDate.dateTextBox.Text;

            if (DateTime.TryParseExact(
                    text,
                    "dd.MM.yyyy",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out DateTime operationDate))
            {

                var response = await suppliesApi.GetAllSuppliesByDateAsync(operationDate);
                if (response.IsSuccessStatusCode && response.Content?.Data is not null)
                {
                    // OperationDate bo‘yicha teskari tartibda (eng so‘nggi birinchi)
                    List<Supply> supplies = [.. response.Content.Data.OrderByDescending(s => s.CreatedAt)];
                    supplyDataGrid.ItemsSource = supplies;
                }
                else
                {
                    MessageBox.Show($"Ta'minotlarni olishda xatolik: {response.Error?.Message ?? "Ma'lumotlar yo'q"}",
                        "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Server bilan ulanishda xatolik: {ex.Message}", "Xatolik",
                MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void tbxRollCount_GotFocus(object sender, RoutedEventArgs e)
    {
        try
        {
            if (cbxProduct.SelectedItem is null && !string.IsNullOrWhiteSpace(cbxProduct.Text))
            {
                string newProductName = cbxProduct.Text.Trim();

                if (cbxProduct.ItemsSource is List<Product> products && !products.Any(p =>
                        p.Name.Equals(newProductName, StringComparison.OrdinalIgnoreCase)))
                {
                    var result = MessageBox.Show(
                        $"\"{newProductName}\" nomli mahsulot mavjud emas.\n" +
                        "Yangi mahsulot yaratmoqchimisiz?",
                        "Yangi mahsulot",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        tbxPerRollCount.Focus(); // Keyingi textboxga o'tadi
                    }
                    else
                    {
                        cbxProduct.Text = null;
                        cbxProduct.Focus(); // Qaytib productga fokus beradi
                    }
                }
            }
        }
        catch { }

        if ((cbxCategory.SelectedItem is null ||
                    cbxCategory.SelectedItem is not null) &&
                    string.IsNullOrWhiteSpace(cbxCategory.Text) &&
                    cbxProduct.SelectedItem is not null)
        {
            var categorytId = (cbxProduct.SelectedItem as Product)!.CategoryId;
            cbxCategory.SelectedItem = _allCategories.FirstOrDefault(a => a.Id == categorytId);
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
                var warehouseItem = await warehouseItemsApi.GetAllWarehouseItemsAsync();
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
                var warehouseItems = await warehouseItemsApi.GetAllWarehouseItemsAsync();
                if (warehouseItems?.Content?.Data is not null)
                {
                    var warehouseItem = warehouseItems.Content.Data.FirstOrDefault(x => x.ProductId == productId);
                    if (warehouseItem is not null)
                    {
                        //tbxPerRollCount.Text = warehouseItem.QuantityPerRoll.ToString("N2");
                        txtPrice.Text = warehouseItem.Price.ToString("N2");
                        tbxDiscountPercent.Text = warehouseItem.DiscountPercent.ToString("N2");
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

            var supply = new Supply
            {
                OperationDate = supplyDate.SelectedDate.Value.ToUniversalTime(),
                CategoryId = categoryId,
                ProductId = productId,
                CountRoll = rollCount,
                QuantityPerRoll = perRollCount,
                TotalQuantity = totalQuantity,
                ProductName = ((Product)cbxProduct.SelectedItem)?.Name ?? cbxProduct.Text ?? string.Empty,
                CategoryName = ((Category)cbxCategory.SelectedItem)?.Name ?? cbxCategory.Text ?? string.Empty,
                Price = price,
                DiscountPercent = discountPercent
            };

            var response = await suppliesApi.CreateSupplyAsync(supply);

            if (response.IsSuccessStatusCode && response.Content is not null)
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
                MessageBox.Show($"Ta'minot qo‘shishda xatolik: {response.Error?.Message ?? "Ma'lumotlar yo‘q"}",
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
            supplyDate.Focus();
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

    private void SupplyDate_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is VoltStream.WPF.Commons.UserControls.UserCalendar uc)
        {
            // Ichidagi dateTextBox’ga fokus berish
            if (uc.FindName("dateTextBox") is TextBox textBox && !textBox.IsKeyboardFocusWithin)
            {
                e.Handled = true;
                textBox.Focus();
                textBox.SelectAll();   // Ixtiyoriy: matnni belgilang
            }
        }
    }

    private async Task<List<Category>> LoadCategoriesAsync()
    {
        try
        {
            var response = await categoriesApi.GetAllCategoriesAsync();
            if (response.IsSuccessStatusCode && response.Content?.Data is not null)
            {
                return response.Content.Data;
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
    private async void cbxCategory_GotFocus(object sender, RoutedEventArgs e)
    {
        _allCategories = await LoadCategoriesAsync();

        categoriesView = CollectionViewSource.GetDefaultView(_allCategories);
        cbxCategory.ItemsSource = categoriesView;

        cbxCategory.IsDropDownOpen = true;
    }
    private void cbxCategory_LostFocus(object sender, RoutedEventArgs e)
    {
        cbxCategory.IsDropDownOpen = false;

    }

    private void cbxCategory_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
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

    private void cbxProduct_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        ComboBoxHelper.BeforeUpdate(sender, e, "Mahsulot nomi", true);
    }
}
