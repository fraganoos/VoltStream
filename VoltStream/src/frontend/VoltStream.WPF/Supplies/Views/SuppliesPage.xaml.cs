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

public partial class SuppliesPage : Page
{
    private readonly IServiceProvider services;
    private readonly IProductsApi productsApi;
    private readonly ICategoriesApi categoriesApi;
    private readonly ISuppliesApi suppliesApi;
    private readonly IWarehouseItemsApi warehouseItemsApi;
    private List<Category> _allCategories = new();
    private ICollectionView _categoriesView;
    public SuppliesPage(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;

        // API xizmatlarini DI orqali olish
        productsApi = services.GetRequiredService<IProductsApi>();
        categoriesApi = services.GetRequiredService<ICategoriesApi>();
        suppliesApi = services.GetRequiredService<ISuppliesApi>();
        warehouseItemsApi = services.GetRequiredService<IWarehouseItemsApi>();
        supplyDate.SelectedDate = DateTime.Now;
        supplyDate.dateTextBox.Focus();
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
                if (response.IsSuccessStatusCode && response.Content?.Data != null)
                {
                    // OperationDate bo‘yicha teskari tartibda (eng so‘nggi birinchi)
                    List<Supply> supplies = response.Content.Data.OrderByDescending(s => s.CreatedAt).ToList();
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

    private void cbxProduct_LostFocus(object sender, RoutedEventArgs e)
    {
        try
        {
            if (cbxProduct.SelectedItem == null && !string.IsNullOrWhiteSpace(cbxProduct.Text))
            {
                string newProductName = cbxProduct.Text.Trim();

                var products = cbxProduct.ItemsSource as List<Product>;
                if (products != null && !products.Any(p =>
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
        catch (Exception ex) { }
        
        if ((cbxCategory.SelectedItem == null|| 
                    cbxCategory.SelectedItem != null) &&
                    string.IsNullOrWhiteSpace(cbxCategory.Text) && 
                    cbxProduct.SelectedItem!=null)
        {
            var categorytId = (cbxProduct.SelectedItem as Product).CategoryId;
            cbxCategory.SelectedItem = _allCategories.FirstOrDefault(a=>a.Id==categorytId);
        }
    }

    private async void tbxRollCount_GotFocus(object sender, RoutedEventArgs e)
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

            if (cbxProduct.SelectedValue != null && long.TryParse(cbxProduct.SelectedValue.ToString(), out long productId))
            {
                var warehouseItems = await warehouseItemsApi.GetAllWarehouseItemsAsync();
                if (warehouseItems?.Content?.Data != null)
                {
                    var warehouseItem = warehouseItems.Content.Data.FirstOrDefault(x => x.ProductId == productId);
                    if (warehouseItem != null)
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

    private async void addSupplyBtn_Click(object sender, RoutedEventArgs e)
    {
        addSupplyBtn.IsEnabled = false;
        try
        {
            // Kiritilgan ma'lumotlarni olish
            if (supplyDate.SelectedDate == null)
            {
                MessageBox.Show("Sana tanlanmagan!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                supplyDate.Focus();
                return;
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

            // Chegirma foizini olish (ixtiyoriy)
            if (!decimal.TryParse(tbxDiscountPercent.Text, out decimal discountPercent) || discountPercent < 0)
            {
                MessageBox.Show("Chegirma foizi noto‘g‘ri kiritilgan!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                tbxDiscountPercent.Focus();
                return;
            }

            // Jami metrni hisoblash
            decimal totalQuantity = perRollCount * rollCount;

            // CategoryId va ProductId ni olish, null bo‘lsa 0 qo‘yiladi
            long categoryId = cbxCategory.SelectedValue != null ? Convert.ToInt64(cbxCategory.SelectedValue) : 0;
            long productId = cbxProduct.SelectedValue != null ? Convert.ToInt64(cbxProduct.SelectedValue) : 0;

            // Supply ob'ektini yaratish
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


            // API orqali ta'minotni saqlash
            var response = await suppliesApi.CreateSupplyAsync(supply);
            System.Diagnostics.Debug.WriteLine($"API javobi: StatusCode={response.StatusCode}, ContentId={response.Content?.Id}, Error={response.Error?.Message}");

            if (response.IsSuccessStatusCode && response.Content != null)
            {

                // Formani tozalash
                cbxCategory.SelectedItem = null;
                cbxCategory.Text = null;
                cbxProduct.SelectedItem = null;
                cbxProduct.Text = null;
                tbxPerRollCount.Text = string.Empty;
                tbxRollCount.Text = string.Empty;
                totalMeters.Text = string.Empty;
                txtPrice.Text = string.Empty;
                tbxDiscountPercent.Text = string.Empty;

                // Kategoriya va mahsulotlarni qayta yuklash
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
            MessageBox.Show($"Server bilan ulanishda xatolik: {ex.Message}", "Xatolik",
                MessageBoxButton.OK, MessageBoxImage.Error);
            System.Diagnostics.Debug.WriteLine($"Xato: {ex.StackTrace}");
        }
        finally
        {
            addSupplyBtn.IsEnabled = true;
            supplyDate.dateTextBox.Focus();
        }
    }

    private async void cbxProduct_GotFocus(object sender, RoutedEventArgs e)
    {
        cbxProduct.ItemsSource = null;
        cbxProduct.IsDropDownOpen = true;
    
        // Hozirgi tanlangan category ni olish
        var selectedCategory = cbxCategory.SelectedItem as Category;

        // 1️⃣ Agar tanlangan category mavjud bo‘lsa
        if (selectedCategory != null && _allCategories.FirstOrDefault(a =>
                a.Name.Equals(cbxCategory.Text.Trim(), StringComparison.OrdinalIgnoreCase)) != null)
        {
            // Shu category ga oid productlarni yuklash
            var products = await productsApi.GetAllProductsByCategoryIdAsync(selectedCategory.Id);

                cbxProduct.ItemsSource = products.Content.Data ?? new List<Product>();
            return;
        }

        // 2️⃣ Agar foydalanuvchi hali hech nima tanlamagan bo‘lsa
        if (cbxCategory.SelectedItem == null &&
            string.IsNullOrWhiteSpace(cbxCategory.Text))
        {
            // Barcha productlarni yuklash
            var allProducts = await productsApi.GetAllProductsAsync();
            cbxProduct.ItemsSource = allProducts.Content.Data ?? new List<Product>();
            return;
        }

        // 2️⃣ Agar foydalanuvchi hali hech nima tanlamagan bo‘lsa 
        if (cbxCategory.SelectedItem != null &&
            string.IsNullOrWhiteSpace(cbxCategory.Text))
        {
            // Barcha productlarni yuklash
            var allProducts = await productsApi.GetAllProductsAsync();
            cbxProduct.ItemsSource = allProducts.Content.Data ?? new List<Product>();
            return;
        }

        // — Product combobox bo‘sh bo‘lib qoladi
        cbxProduct.ItemsSource = new List<Product>();
}


    private void txtPrice_GotFocus(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox tb)
        {
            tb.Dispatcher.BeginInvoke(new Action(tb.SelectAll));
        }
    }

    private void txtPrice_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        var tb = sender as TextBox;
        if (tb != null && !tb.IsKeyboardFocusWithin)
        {
            e.Handled = true;
            tb.Focus();
        }
    }

    private void tbxDiscountPercent_GotFocus(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox tb)
        {
            tb.Dispatcher.BeginInvoke(new Action(tb.SelectAll));
        }
    }

    private void tbxDiscountPercent_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        var tb = sender as TextBox;
        if (tb != null && !tb.IsKeyboardFocusWithin)
        {
            e.Handled = true;
            tb.Focus();
        }
    }

    private async void supplyDate_LostFocus(object sender, RoutedEventArgs e)
    {
        await LoadSuppliesAsync();
    }

    private async void supplyDate_GotFocus(object sender, RoutedEventArgs e)
    {
        await LoadSuppliesAsync();
    }

    private void supplyDate_PreviewMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is VoltStream.WPF.Commons.UserControls.UserCalendar uc)
        {
            // Ichidagi dateTextBox’ga fokus berish
            var textBox = uc.FindName("dateTextBox") as TextBox;
            if (textBox != null && !textBox.IsKeyboardFocusWithin)
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
            if (response.IsSuccessStatusCode && response.Content?.Data != null)
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
        return new List<Category>();
    }

    private async void cbxCategory_GotFocus(object sender, RoutedEventArgs e)
    {
        if (_allCategories.Count == 0)
            _allCategories = await LoadCategoriesAsync();

        _categoriesView = CollectionViewSource.GetDefaultView(_allCategories);
        cbxCategory.ItemsSource = _categoriesView;
        
        cbxCategory.IsDropDownOpen = true;
    }

    private TextBox _categoryTextBox;
    private void cbxCategory_Loaded(object sender, RoutedEventArgs e)
    {
        // ComboBox yuklanganda ichidagi TextBox’ni topamiz
        _categoryTextBox = cbxCategory.Template.FindName("PART_EditableTextBox", cbxCategory) as TextBox;
        if (_categoryTextBox != null)
            _categoryTextBox.TextChanged -= CategoryTextBox_TextChanged;
    }

    private void CategoryTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        string currentText = _categoryTextBox.Text; // shu paytdagi yozilgan matn
        cbxCategory.ItemsSource = _allCategories.Where(a => a.Name.Contains(currentText));                                            // kerakli ishlarni shu yerda qilasan (filter, log va h.k.)
        cbxCategory.IsDropDownOpen = true;
    }

    private void cbxCategory_LostFocus(object sender, RoutedEventArgs e)
    {
        cbxCategory.IsDropDownOpen = false;
    }

   
}
