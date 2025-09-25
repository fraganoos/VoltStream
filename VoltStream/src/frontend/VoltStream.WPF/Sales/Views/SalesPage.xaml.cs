namespace VoltStream.WPF.Sales.Views;

using ApiServices.DTOs.Customers;
using ApiServices.DTOs.Products;
using ApiServices.DTOs.Supplies;
using ApiServices.Interfaces;
using ApiServices.Models;
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
        CustomerName.PreviewLostKeyboardFocus += (s, e) => ComboBoxHelper.BeforeUpdate(s, e, "Mijoz");
        CustomerName.LostFocus += CustomerName_LostFocus;

        cbxCategoryName.GotFocus += CbxCategoryName_GotFocus;
        cbxCategoryName.PreviewLostKeyboardFocus += CbxCategoryName_PreviewLostKeyboardFocus;

        cbxProductName.GotFocus += CbxProductName_GotFocus;
        cbxProductName.SelectionChanged += CbxProductName_SelectionChanged;
        cbxProductName.PreviewLostKeyboardFocus += CbxProductName_PreviewLostKeyboardFocus;
        cbxProductName.LostFocus += CbxProductName_LostFocus;

        cbxPerRollCount.GotFocus += CbxPerRollCount_GotFocus;
        cbxPerRollCount.SelectionChanged += CbxPerRollCount_SelectionChanged;
        cbxPerRollCount.PreviewLostKeyboardFocus += CbxPerRollCount_PreviewLostKeyboardFocus;

        txtRollCount.LostFocus += (s, e) => CalcFinalSumProduct(s);
        txtQuantity.PreviewLostKeyboardFocus += TxtQuantity_PreviewLostKeyboardFocus;
        txtPrice.LostFocus += (s, e) => CalcFinalSumProduct(s);
        txtSum.LostFocus += TxtSum_LostFocus;
        txtPerDiscount.PreviewLostKeyboardFocus += txtPerDiscount_PreviewLostKeyboardFocus;
        txtDiscount.PreviewLostKeyboardFocus += txtDiscount_PreviewLostKeyboardFocus;
        txtFinalSumProduct.PreviewLostKeyboardFocus += txtFinalSumProduct_PreviewLostKeyboardFocus;
    }

    private async void CustomerName_LostFocus(object sender, RoutedEventArgs e)
    {
        long? customerId = null;
        if (CustomerName.SelectedValue!=null)
        {
            customerId = (long)CustomerName.SelectedValue;
        }
        else
        {
            beginBalans.Clear();
            lastBalans.Text = null;
            tel.Text=null;
            return;
        }
        await LoadCustomerByIdAsync(customerId);
    }
    private async Task LoadCustomerByIdAsync(long? customerId)
    {
        try
        {
            if (customerId.HasValue && customerId.Value != 0)
            {
                var response = await customersApi.GetCustomerByIdAsync(customerId.Value);
                if (response.IsSuccessStatusCode && response.Content?.Data != null)
                {
                    var accounts = response.Content.Data.Accounts;
                    beginBalans.Text = accounts.CurrentSumm.ToString("N2");
                    tel.Text=response.Content.Data.Phone;
                    decimal beginSumm = 0;
                    decimal saleSumm = 0;
                    if (decimal.TryParse(beginBalans.Text, out decimal value)) beginSumm = value;
                    if (decimal.TryParse(finalSumm.Text, out decimal amount)) saleSumm = amount;
                    decimal endSumm = beginSumm - saleSumm;
                    lastBalans.Text = endSumm.ToString("N2");



                    //sale.CustomerId = customer.Id;
                    //sale.CustomerName = customer.Name;
                }
                else
                {
                    var errorMsg = response.Error?.Message ?? "Unknown error";
                    MessageBox.Show("Error fetching customer by ID: " + errorMsg);
                }
            }
            else
            {
                // Если customerId не задан, очищаем поля
                //sale.CustomerId = null;
                //sale.CustomerName = string.Empty;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("An error occurred: " + ex.Message);
        }
    }

    private async void CustomerName_GotFocus(object sender, RoutedEventArgs e) 
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

    private void txtFinalSumProduct_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        if(decimal.TryParse(txtFinalSumProduct.Text, out decimal finalSum) &&
            decimal.TryParse(txtSum.Text, out decimal sum) && sum != 0)
        {
            if (finalSum > sum)
            {
                MessageBox.Show("Chegirmadan keyingi summa umumiy summadan katta bo'lishi mumkin emas.", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                txtPerDiscount.Text = null;
                txtDiscount.Text = null;
                txtFinalSumProduct.Text = null;
                e.Handled = true; // Fokusni saqlab qolish
                return;
            }
            decimal discount = sum - finalSum;
            decimal perDiscount = (discount / sum * 100);
            txtDiscount.Text = discount.ToString("N2");
            txtPerDiscount.Text = perDiscount.ToString("N2");
        }
        else
        {
            txtFinalSumProduct.Text = null;
            txtPerDiscount.Text = "0";
            CalcFinalSumProduct(sender);
        }
    }

    private void txtDiscount_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (decimal.TryParse(txtDiscount.Text, out decimal discount) &&
            decimal.TryParse(txtSum.Text, out decimal sum) && sum != 0)
        {
            if (discount>sum)
            {
             MessageBox.Show("Chegirma umumiy summadan katta bo'lishi mumkin emas.", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                txtPerDiscount.Text = null;
                txtDiscount.Text = null;
                txtFinalSumProduct.Text = null;
                e.Handled = true; // Fokusni saqlab qolish
                return;
            }
            decimal perDiscount = (discount / sum *100);
            txtFinalSumProduct.Text = (sum - discount).ToString("N2");
            txtPerDiscount.Text = perDiscount.ToString("N2");
        }
        else
        {
            txtDiscount.Text = "0";
            txtPerDiscount.Text = "0";
            CalcFinalSumProduct(sender);
        }
    }


    private void txtPerDiscount_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (decimal.TryParse(txtPerDiscount.Text, out decimal perDiscount) &&
            perDiscount != 0)
        {
            if (perDiscount >= 100)
            {
                MessageBox.Show("Chegirma 100% dan katta bo'lishi mumkin emas.", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                txtPerDiscount.Text = null;
                txtDiscount.Text = null;
                txtFinalSumProduct.Text = null;
                e.Handled = true; // Fokusni saqlab qolish
                return;
            }
            CalcFinalSumProduct(sender);
        }
        else
        {
            txtPerDiscount.Text = "0";
            CalcFinalSumProduct(sender);
        }
    }

    private void TxtSum_LostFocus(object sender, RoutedEventArgs e)
    {
            if (decimal.TryParse(txtSum.Text, out decimal sum) &&
            decimal.TryParse(txtQuantity.Text, out decimal quantity) && quantity !=0)
        {
            decimal price = sum / quantity;
            txtPrice.Text = price.ToString("N2");
            CalcFinalSumProduct(sender);
        }
        else
        {
            txtSum.Text = null;
            txtFinalSumProduct.Text = null;
        }
    }


    private void TxtQuantity_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (decimal.TryParse(txtQuantity.Text, out decimal quantity) &&
            decimal.TryParse(cbxPerRollCount.Text, out decimal perRollCount) &&
            perRollCount != 0)
        {
            decimal rollCount = Math.Ceiling(quantity / perRollCount);
            txtRollCount.Text = rollCount.ToString("N2");
            CalcFinalSumProduct(sender);
        }
        else
        {
            txtQuantity.Text = null;
            txtSum.Text = null;
            txtFinalSumProduct.Text = null;
        }
    }

    private void CalcFinalSumProduct(object sender)
    {
        if (sender == cbxPerRollCount || sender == txtRollCount)
        {
            if (decimal.TryParse(txtRollCount.Text, out decimal rollCount) &&
            decimal.TryParse(cbxPerRollCount.Text, out decimal perRollCount))
            {
                // Umumiy uzunlik hisoblaymiz
                decimal totalQuantity = rollCount * perRollCount;
                txtQuantity.Text = totalQuantity.ToString("N2");
            }
            else
            {
                txtQuantity.Text = null;
                txtSum.Text = null;
                txtFinalSumProduct.Text = null;
            }
        }
        if (decimal.TryParse(txtPrice.Text, out decimal price) &&
            decimal.TryParse(txtQuantity.Text, out decimal quantity) &&
            decimal.TryParse(txtPerDiscount.Text, out decimal discountPercent))
        {
            // Umumiy narx hisoblaymiz
            decimal totalPrice = price * quantity;
            decimal discountAmount = totalPrice * (discountPercent / 100);
            decimal finalPrice = totalPrice - discountAmount;
            txtSum.Text = totalPrice.ToString("N2");
            txtDiscount.Text = discountAmount.ToString("N2");
            txtFinalSumProduct.Text = finalPrice.ToString("N2");
        }
        else
        {
            txtSum.Text = null;
            txtDiscount.Text = null;
            txtFinalSumProduct.Text = null;
        }
    }
    private void CbxPerRollCount_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (cbxPerRollCount.SelectedItem is WarehouseItem selectedWarehouseItem)
        {
            // Rulon tanlanganda, uning narxi, chegirmasi o'zgartiramiz
            txtPrice.Text = selectedWarehouseItem.Price.ToString("N2");
            txtPerDiscount.Text = selectedWarehouseItem.DiscountPercent.ToString("N2");
            CalcFinalSumProduct(sender);
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
        try
        {
            cbxPerRollCount.IsDropDownOpen = true;
        }
        catch (Exception ex)
        {
            //ignored
        }
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

    private void addButton_Click(object sender, RoutedEventArgs e)
    {
        SaleItem saleItem = new SaleItem()
        {
            CategoryId = cbxCategoryName.SelectedIndex,
            CategoryName = cbxCategoryName.Text,
            ProductId = cbxProductName.SelectedIndex,
            ProductName = cbxProductName.Text,
            PerRollCount = decimal.TryParse(cbxPerRollCount.Text, out decimal perRollCount) ? perRollCount : 0,
            RollCount = decimal.TryParse(txtRollCount.Text, out decimal rollCount) ? rollCount : 0,
            Quantity = decimal.TryParse(txtQuantity.Text, out decimal quantity) ? quantity : 0,
            Price = decimal.TryParse(txtPrice.Text, out decimal price) ? price : 0,
            Sum = decimal.TryParse(txtSum.Text, out decimal sum) ? sum : 0,
            PerDiscount = decimal.TryParse(txtPerDiscount.Text, out decimal perDiscount) ? perDiscount : 0,
            Discount = decimal.TryParse(txtDiscount.Text, out decimal discount) ? discount : 0,
            FinalSumProduct = decimal.TryParse(txtFinalSumProduct.Text, out decimal finalSumProduct) ? finalSumProduct : 0
        };
        sale.SaleItems.Insert(0, saleItem);
        sale.FinalSum=sale.SaleItems.Sum(s => s.Sum);

        cbxCategoryName.SelectedValue = null;
        cbxProductName.SelectedValue = null;
        cbxPerRollCount.SelectedValue = null;
        txtRollCount.Clear();
        txtQuantity.Clear();
        txtPrice.Clear();
        txtSum.Clear();
        txtPerDiscount.Clear();
        txtDiscount.Clear();
        txtFinalSumProduct.Clear();
        //MessageBox.Show(sale.FinalSum.ToString());
        cbxCategoryName.Focus();
    }
}

