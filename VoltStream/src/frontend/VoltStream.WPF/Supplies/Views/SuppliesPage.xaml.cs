namespace VoltStream.WPF.Supplies.Views;

using ApiServices.DTOs.Products;
using ApiServices.DTOs.Supplies;
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
        await LoadSuppliesAsync(); // DataGrid ni dastlabki yuklash
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

    private async Task LoadSuppliesAsync()
    {
        try
        {
            var response = await suppliesApi.GetAllSuppliesAsync();
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

    private async void cbxCategory_LostFocus(object sender, RoutedEventArgs e)
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
                        await LoadProductsAsync();
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
                long categoryId = Convert.ToInt64(cbxCategory.SelectedValue);
                var products = await categoriesApi.GetByIdCategoryAsync(categoryId);

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
            }
        }
        finally
        {
            isProcessingProductLostFocus = false; // Hodisa tugadi, flagni qayta tiklaymiz
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

            // Yuborilayotgan ma'lumotlarni log qilish
            System.Diagnostics.Debug.WriteLine($"Yuborilayotgan Supply: CategoryId={supply.CategoryId}, ProductId={supply.ProductId}, ProductName={supply.ProductName}, TotalQuantity={supply.TotalQuantity}, Price={supply.Price}");

            // API orqali ta'minotni saqlash
            var response = await suppliesApi.CreateSupplyAsync(supply);
            System.Diagnostics.Debug.WriteLine($"API javobi: StatusCode={response.StatusCode}, ContentId={response.Content?.Id}, Error={response.Error?.Message}");

            if (response.IsSuccessStatusCode && response.Content != null)
            {
                MessageBox.Show($"Ta'minot muvaffaqiyatli qo‘shildi! ID: {response.Content.Id}",
                    "Muvaffaqiyat", MessageBoxButton.OK, MessageBoxImage.Information);

                // Formani tozalash
                supplyDate.SelectedDate = null;
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
                await LoadCategoriesAsync();
                await LoadProductsAsync();
                await LoadSuppliesAsync(); // DataGrid ni yangilash
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
    }

    //private async Task LoadProductsByCategoryIDAsync(long categoryId)
    //{
    //    try
    //    {
    //        // categoryId validligini tekshirish
    //        if (categoryId <= 0)
    //        {
    //            MessageBox.Show("Noto‘g‘ri kategoriya ID.", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
    //            return;
    //        }

    //        // API orqali kategoriya ma'lumotlarini olish
    //        var response = await categoriesApi.GetByIdCategoryAsync(categoryId);
    //        if (response.IsSuccessStatusCode && response.Content?.Data != null)
    //        {
    //            // CategoryDto dan Products ro‘yxatini olish
    //            var category = response.Content.Data;
    //            List<Product> products = category.Products?.ToList() ?? new List<Product>();

    //            // cbxProduct ga ma'lumotlarni o‘rnatish
    //            cbxProduct.ItemsSource = products;
    //            cbxProduct.DisplayMemberPath = "Name";
    //            cbxProduct.SelectedValuePath = "Id";

    //            // Mahsulotlar ro‘yxati bo‘sh bo‘lsa
    //            if (products.Count == 0)
    //            {
    //                MessageBox.Show("Mahsulotlar topilmadi.", "Ma'lumot", MessageBoxButton.OK, MessageBoxImage.Information);
    //            }
    //        }
    //        else
    //        {
    //            // Xatolik xabari
    //            MessageBox.Show($"Mahsulotlar olishda xatolik: {response.Error?.Message ?? "Ma'lumotlar yo‘q"}",
    //                "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        // Umumiy xatolikni log qilish va foydalanuvchiga ko‘rsatish
    //        MessageBox.Show($"Server bilan ulanishda xatolik: {ex.Message}", "Xatolik",
    //            MessageBoxButton.OK, MessageBoxImage.Error);
    //        System.Diagnostics.Debug.WriteLine($"Xato: {ex.Message}\nStackTrace: {ex.StackTrace}");
    //    }
    //}
    private async void cbxCategory_GotFocus(object sender, RoutedEventArgs e)
    {
        await LoadCategoriesAsync();
    }
}