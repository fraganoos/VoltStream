namespace VoltStream.WPF.Supplies.Views;


using ApiServices.DTOs.Products;
using ApiServices.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

public partial class SuppliesPage : Page
{
    private readonly IServiceProvider services;
    private readonly IProductsApi productsApi;
    private readonly ICategoriesApi categoriesApi;
    private readonly ISuppliesApi suppliesApi;
    private readonly IWarehouseItemsApi warehouseItemsApi;
    private bool isProcessingProductLostFocus = false; // Hodisani qayta ishlashni oldini olish uchun flag
    private bool isProcessingCategoryLostFocus = false; // Hodisani qayta ishlashni oldini olish uchun flag



    public SuppliesPage(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;

        // API xizmatlarini DI orqali olish
        productsApi = services.GetRequiredService<IProductsApi>();
        categoriesApi = services.GetRequiredService<ICategoriesApi>();
        suppliesApi = services.GetRequiredService<ISuppliesApi>();
        warehouseItemsApi = services.GetRequiredService<IWarehouseItemsApi>();
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

    private void cbxCategory_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter || e.Key == Key.Tab)
        {
            // Agar element tanlangan bo'lsa, to'g'ridan-to'g'ri cbxProduct ga o'tamiz
            if (cbxCategory.SelectedItem != null)
            {
                cbxProduct.Focus();
                e.Handled = true; // Hodisani to'xtatamiz, qayta LostFocus chaqirilmasligi uchun
            }
            else
            {
                // Agar element tanlanmagan bo'lsa, LostFocus hodisasini ishga tushiramiz
                cbxCategory_LostFocus(sender, new RoutedEventArgs());
            }
        }
    }

    private void cbxCategory_LostFocus(object sender, RoutedEventArgs e)
    {
        // Agar hodisa allaqachon ishlayotgan bo'lsa, qayta ishlashni to'xtatamiz
        if (isProcessingCategoryLostFocus)
            return;

        isProcessingCategoryLostFocus = true; // Hodisani boshladik

        try
        {
            if (cbxCategory.SelectedItem == null && !string.IsNullOrWhiteSpace(cbxCategory.Text))
            {
                string newCategoryName = cbxCategory.Text.Trim().ToLower();

                // ItemsSource ni Category listiga cast qilamiz
                var categories = cbxCategory.ItemsSource as List<Category>;
                if (categories != null && !categories.Any(c =>
                        c.Name.ToLower().Equals(newCategoryName, StringComparison.OrdinalIgnoreCase)))
                {
                    // Ha yoki Yo‘q so‘rovi
                    var result = MessageBox.Show(
                        $"\"{newCategoryName}\" nomli kategoriya mavjud emas.\n" +
                        "Yangi kategoriya yaratmoqchimisiz?",
                        "Yangi kategoriya",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question);

                    if (result == MessageBoxResult.Yes)
                    {
                        cbxProduct.Focus();
                    }
                    else if (result == MessageBoxResult.No)
                    {
                        cbxCategory.Text = null;
                        cbxCategory.Focus();
                    }
                }
                else
                {
                    // Agar kategoriya matni mavjud elementlardan biriga mos kelsa
                    cbxProduct.Focus();
                }
            }
            else if (cbxCategory.SelectedItem != null)
            {
                // Agar element tanlangan bo'lsa, to'g'ridan-to'g'ri cbxProduct ga o'tamiz
                cbxProduct.Focus();
            }
        }
        finally
        {
            isProcessingCategoryLostFocus = false; // Hodisa tugadi, flagni qayta tiklaymiz
        }
    }



    private void cbxProduct_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter || e.Key == Key.Tab)
        {
            // Agar element tanlangan bo'lsa, to'g'ridan-to'g'ri tbxPerRollCount ga o'tamiz
            if (cbxProduct.SelectedItem != null)
            {
                tbxPerRollCount.Focus();
                e.Handled = true; // Hodisani to'xtatamiz, qayta LostFocus chaqirilmasligi uchun
            }
            else
            {
                // Agar element tanlanmagan bo'lsa, LostFocus hodisasini ishga tushiramiz
                cbxProduct_LostFocus(sender, new RoutedEventArgs());
            }
        }
    }

    private async void cbxProduct_LostFocus(object sender, RoutedEventArgs e)
    {
        // Agar hodisa allaqachon ishlayotgan bo'lsa, qayta ishlashni to'xtatamiz
        if (isProcessingProductLostFocus)
            return;

        isProcessingProductLostFocus = true; // Hodisani boshladik

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
                        var warehouseItems = await warehouseItemsApi.GetAllWarehouseItemsAsync();
                    }
                    else
                    {
                        cbxProduct.Text = null;
                        cbxProduct.Focus(); // Qaytib productga fokus beradi
                    }
                }

            }
            else if (cbxProduct.SelectedItem != null)
            {
                // Agar element tanlangan bo'lsa, to'g'ridan-to'g'ri tbxPerRollCount ga o'tamiz
                tbxPerRollCount.Focus();
                long productId = (long)cbxProduct.SelectedValue;
                var warehouseItems = await warehouseItemsApi.GetAllWarehouseItemsAsync();
                if (warehouseItems?.Content?.Data is not null)
                {
                    var warehouseItem = warehouseItems.Content.Data.FirstOrDefault(x => x.ProductId == productId);
                    if (warehouseItem != null)
                    {
                        tbxPerRollCount.Text = warehouseItem.QuantityPerRoll.ToString("N2");
                        txtPrice.Text = warehouseItem.Price.ToString("N2");
                        tbxDiscountPercent.Text = warehouseItem.DiscountPercent.ToString("N2");
                    }
                }
            }
        }
        finally
        {
            isProcessingProductLostFocus = false; // Hodisa tugadi, flagni qayta tiklaymiz
        }
    }

    private void tbxRollCount_GotFocus(object sender, RoutedEventArgs e)
    {
        try
        {
            // tbxPerRollCount va tbxRollCount qiymatlarini olish
            if (decimal.TryParse(tbxPerRollCount.Text, out decimal perRollCount) &&
                decimal.TryParse(tbxRollCount.Text, out decimal rollCount))
            {
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
        }
        catch (Exception ex)
        {
            // Xatolik yuzaga kelsa, log yozish yoki xabar ko‘rsatish
            System.Diagnostics.Debug.WriteLine($"Xato: {ex.Message}");
            totalMeters.Text = string.Empty;
        }
    }
    private void addSupplyBtn_Click(object sender, RoutedEventArgs e)
    {
        // Bu yerda keyinchalik qo‘shish funksiyasi yoziladi
    }
}
