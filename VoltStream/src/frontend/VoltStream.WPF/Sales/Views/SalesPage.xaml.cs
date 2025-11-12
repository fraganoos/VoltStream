namespace VoltStream.WPF.Sales.Views;

using ApiServices.Extensions;
using ApiServices.Interfaces;
using ApiServices.Models;
using ApiServices.Models.Requests;
using ApiServices.Models.Responses;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VoltStream.WPF.Commons;
using VoltStream.WPF.Customer;
using VoltStream.WPF.Sales.ViewModels;

/// <summary>
/// Логика взаимодействия для SalesPage.xaml
/// </summary>
public partial class SalesPage : Page
{
    private readonly IServiceProvider services;
    private readonly ICategoriesApi categoriesApi;
    private readonly IProductsApi productsApi;
    private readonly IWarehouseStocksApi warehouseItemsApi;
    private readonly ICustomersApi customersApi;
    private readonly ICurrenciesApi currenciesApi;
    private readonly ISaleApi salesApi;

    public Sale sale = new();

    public SalesPage(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
        DataContext = sale;
        categoriesApi = services.GetRequiredService<ICategoriesApi>();
        productsApi = services.GetRequiredService<IProductsApi>();
        warehouseItemsApi = services.GetRequiredService<IWarehouseStocksApi>();
        customersApi = services.GetRequiredService<ICustomersApi>();
        salesApi = services.GetRequiredService<ISaleApi>();
        currenciesApi = services.GetRequiredService<ICurrenciesApi>();

        CustomerName.GotFocus += CustomerName_GotFocus;

        CustomerName.PreviewLostKeyboardFocus += CustomerName_PreviewLostKeyboardFocus;
        CustomerName.LostFocus += CustomerName_LostFocus;

        cbxCategoryName.GotFocus += CbxCategoryName_GotFocus;
        cbxCategoryName.PreviewLostKeyboardFocus += CbxCategoryName_PreviewLostKeyboardFocus;

        cbxProductName.GotFocus += CbxProductName_GotFocus;
        cbxProductName.SelectionChanged += CbxProductName_SelectionChanged;
        cbxProductName.PreviewLostKeyboardFocus += CbxProductName_PreviewLostKeyboardFocus;
        cbxProductName.LostFocus += CbxProductName_LostFocus;

        //cbxPerRollCount.GotFocus += CbxPerRollCount_GotFocus;
        cbxPerRollCount.SelectionChanged += CbxPerRollCount_SelectionChanged;
        cbxPerRollCount.PreviewLostKeyboardFocus += CbxPerRollCount_PreviewLostKeyboardFocus;

        txtRollCount.LostFocus += (s, e) => CalcFinalSumProduct(s);
        txtRollCount.PreviewLostKeyboardFocus += TxtRollCount_PreviewLostKeyboardFocus;
        txtQuantity.PreviewLostKeyboardFocus += TxtQuantity_PreviewLostKeyboardFocus;
        txtPrice.LostFocus += (s, e) => CalcFinalSumProduct(s);
        txtSum.LostFocus += TxtSum_LostFocus;
        txtPerDiscount.PreviewLostKeyboardFocus += TxtPerDiscount_PreviewLostKeyboardFocus;
        txtDiscount.PreviewLostKeyboardFocus += TxtDiscount_PreviewLostKeyboardFocus;
        txtFinalSumProduct.PreviewLostKeyboardFocus += TxtFinalSumProduct_PreviewLostKeyboardFocus;


        CurrencyType.SelectionChanged += CurrencyType_SelectionChanged;

    }

    private void CurrencyType_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        sale.CurrencyId = CurrencyType.SelectedValue is not null ? (long)CurrencyType.SelectedValue : 0;
    }

    private void TxtRollCount_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (txtRollCount.Text.Length > 0)
        {
            if ((decimal.TryParse(txtRollCount.Text, out decimal value) ? value : 0) > sale.WarehouseCountRoll)
            {
                if (MessageBox.Show($"Omborda {cbxProductName.Text} -dan {sale.WarehouseCountRoll} " +
                    $"rulon qolgan." + Environment.NewLine + "Davom ettirishga rozimisiz?",
                    "Savdo", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                {
                    e.Handled = true;
                    txtRollCount.Text = null;
                }
            }
        }
    }

    private async void CustomerName_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {

        bool accept = ComboBoxHelper.BeforeUpdate(sender, e, "Xaridor", true);
        if (accept)
        {
            var win = new CustomerWindow(CustomerName.Text);
            if (win.ShowDialog() == true)
            {
                var customer = win.Result;
                CustomerRequest newCustomer = new()
                {
                    Name = customer!.name,
                    Phone = customer.phone,
                    Address = customer.address,
                    Description = customer.description,
                    Accounts = [new()
                    {
                        OpeningBalance = customer.beginningSum,
                        Balance = customer.beginningSum,
                        Discount = 0,
                        CurrencyId = (long)CurrencyType.SelectedValue 
                    }]
                };

                var response = await customersApi.CreateAsync(newCustomer).Handle();
                if (response.IsSuccess)
                {
                    CustomerName.Text = newCustomer.Name;
                    await LoadCustomerNameAsync();
                }
                else
                {
                    e.Handled = true;
                    MessageBox.Show($"Xatolik yuz berdi. {response.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                // тут можете сохранить customer в БД или список
            }
            else { e.Handled = true; }
        }
    }

    private async void CustomerName_LostFocus(object sender, RoutedEventArgs e)
    {
        if (CustomerName.SelectedValue is not null)
        {
            sale.CustomerId = (long)CustomerName.SelectedValue;
        }
        else
        {
            beginBalans.Clear();
            lastBalans.Text = null;
            tel.Text = null;
            return;
        }
        await LoadCustomerByIdAsync(sale.CustomerId);
    }

    private async Task LoadCustomerByIdAsync(long? customerId)
    {
        try
        {
            if (customerId.HasValue && customerId.Value != 0)
            {
                FilteringRequest request = new()
                {
                    Filters = new()
                    {
                        ["id"] = [customerId.Value.ToString()],
                        ["accounts"] = ["include:currency"]
                    }
                };

                var response = await customersApi.Filter(request).Handle();
                if (response.IsSuccess)
                {
                    var customer = response.Data!.First();

                    beginBalans.Text = GetAccountsSumInUzsString(customer);
                    tel.Text = customer.Phone;
                    CalcSaleSum();
                }
                else
                {
                    var errorMsg = response.Message ?? "Unknown error";
                    sale.Error = errorMsg;
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


    private static string GetAccountsSumInUzsString(CustomerResponse customer)
    {
        if (customer?.Accounts == null) return "0";

        decimal totalUzs = 0m;
        foreach (var acc in customer.Accounts)
        {
            if (acc == null) continue;
            var rate = acc.Currency?.ExchangeRate ?? 1m;
            if (rate == 0m) rate = 1m;
            totalUzs += acc.Balance * rate;
        }

        var rounded = Math.Round(totalUzs, 0, MidpointRounding.AwayFromZero);
        return rounded.ToString("F0");
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
            var response = await customersApi.GetAllAsync();

            if (response.IsSuccess)
            {
                List<CustomerResponse> customers = response.Data!;
                CustomerName.ItemsSource = customers;
                CustomerName.DisplayMemberPath = "Name";
                CustomerName.SelectedValuePath = "Id";
                // Восстанавливаем выбранное значение
                if (selectedValue is not null)
                    CustomerName.SelectedValue = selectedValue;
            }
            else
            {
                // Проверяем на null, чтобы избежать CS8602
                var errorMsg = response.Message ?? "Unknown error";
                MessageBox.Show("Error fetching customers: " + errorMsg);
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show("An error occurred: " + ex.Message);
        }
    }

    private void TxtFinalSumProduct_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (decimal.TryParse(txtFinalSumProduct.Text, out decimal finalSum) &&
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

    private void TxtDiscount_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        if (decimal.TryParse(txtDiscount.Text, out decimal discount) &&
            decimal.TryParse(txtSum.Text, out decimal sum) && sum != 0)
        {
            if (discount > sum)
            {
                MessageBox.Show("Chegirma umumiy summadan katta bo'lishi mumkin emas.", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                txtPerDiscount.Text = null;
                txtDiscount.Text = null;
                txtFinalSumProduct.Text = null;
                e.Handled = true; // Fokusni saqlab qolish
                return;
            }
            decimal perDiscount = (discount / sum * 100);
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


    private void TxtPerDiscount_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
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
        decimal.TryParse(txtQuantity.Text, out decimal quantity) && quantity != 0)
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
            if ((decimal.TryParse(txtQuantity.Text, out decimal value) ? value : 0) > sale.WarehouseQuantity)
            {
                if (MessageBox.Show($"Omborda {cbxProductName.Text}-ning {cbxPerRollCount.Text}-metrligidan {sale.WarehouseQuantity} " +
                    $"metr qolgan." + Environment.NewLine + "Davom ettirishga rozimisiz?",
                    "Savdo", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.No)
                {
                    e.Handled = true;
                    txtQuantity.Text = null;
                    return;
                }
            }
            if (quantity % perRollCount != 0)
            {
                int intRollCount = (int)(quantity / perRollCount);
                decimal _q = (decimal)intRollCount * perRollCount;
                decimal _quantity = quantity - _q;
                decimal q_quantity = perRollCount - _quantity;
                sale.NewQuantity = q_quantity;
                if (MessageBox.Show($"Siz {cbxProductName.Text}-ning {cbxPerRollCount.Text}-metrlik rulonidan " +
                    $"{quantity} metr tanladingiz, shunda bitta rulondan {_quantity} metr " +
                    $"kesilib omborga bitta {q_quantity} metrlik rulon qo'shiladi." + Environment.NewLine +
                    "Davom ettirishga rozimisiz?", "Savdo", MessageBoxButton.YesNo, MessageBoxImage.Question,
                    MessageBoxResult.No) == MessageBoxResult.No)
                {
                    e.Handled = true;
                    txtQuantity.Text = _q.ToString();
                    sale.NewQuantity = 0;
                    txtQuantity.SelectAll();
                    return;
                }
            }

            decimal rollCount = Math.Ceiling(quantity / perRollCount);
            txtRollCount.Text = rollCount.ToString();
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
        if (cbxPerRollCount.SelectedItem is WarehouseStockResponse selectedWarehouseItem)
        {
            // Rulon tanlanganda, uning narxi, chegirmasi o'zgartiramiz
            txtPrice.Text = selectedWarehouseItem.UnitPrice.ToString();
            txtPerDiscount.Text = selectedWarehouseItem.DiscountRate.ToString();
            sale.WarehouseCountRoll = selectedWarehouseItem.RollCount;
            sale.WarehouseQuantity = selectedWarehouseItem.TotalLength;
            CalcFinalSumProduct(sender);
        }
    }

    private async void CbxCategoryName_GotFocus(object sender, RoutedEventArgs e)
    {
        await LoadCategoryAsync();
        //cbxCategoryName.IsDropDownOpen = true;
    }
    private void CbxCategoryName_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        _ = ComboBoxHelper.BeforeUpdate(sender, e, "Maxsulot turi");
    }

    private async void CbxProductName_GotFocus(object sender, RoutedEventArgs e)
    {
        long? categoryId = null;
        if (cbxCategoryName.SelectedValue is not null)
        {
            categoryId = (long)cbxCategoryName.SelectedValue;
        }
        await LoadProductAsync(categoryId);
        //cbxProductName.IsDropDownOpen = true;
    }
    private void CbxProductName_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (cbxProductName.SelectedItem is ProductResponse selectedProduct)
        {
            // maxsulot tanlanganda, uning categoryId sini ham olamiz va cbxCategoryName dagini o'zgartiramiz
            cbxCategoryName.SelectedValue = selectedProduct.Category.Id;
            sale.CategoryId = selectedProduct.Category.Id;
            sale.CategoryName = (cbxCategoryName.ItemsSource as IEnumerable<CategoryResponse>)?
                .FirstOrDefault(c => c.Id == selectedProduct.Category.Id)?.Name ?? string.Empty;
            sale.ProductId = selectedProduct.Id;
        }
    }
    private void CbxProductName_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        ComboBoxHelper.BeforeUpdate(sender, e, "Maxsulot");
    }
    private async void CbxProductName_LostFocus(object sender, RoutedEventArgs e)
    {
        long? productId = null;
        if (cbxProductName.SelectedValue is not null)
        {
            productId = (long)cbxProductName.SelectedValue;
        }
        await LoadWarehouseItemsAsync(productId);
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

            var response = await categoriesApi.GetAllAsync().Handle();
            if (response.IsSuccess)
            {
                List<CategoryResponse> categories = response.Data!;
                cbxCategoryName.ItemsSource = categories;
                cbxCategoryName.DisplayMemberPath = "Name";
                cbxCategoryName.SelectedValuePath = "Id";

                // Восстанавливаем выбранное значение
                if (selectedValue is not null)
                    cbxCategoryName.SelectedValue = selectedValue;
            }
            else
            {
                // Проверяем на null, чтобы избежать CS8602
                var errorMsg = response.Message ?? "Unknown error";
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
            Response<List<ProductResponse>> response;

            FilteringRequest request = new()
            {
                Filters = new()
                {
                    ["category"] = ["include"]
                }
            };

            if (categoryId.HasValue && categoryId.Value != 0)
                request.Filters["CategoryId"] = [categoryId.Value.ToString()];

            response = await productsApi.Filter(request).Handle();

            if (response.IsSuccess)
            {
                var products = response.Data;
                cbxProductName.ItemsSource = products;
                cbxProductName.DisplayMemberPath = "Name";
                cbxProductName.SelectedValuePath = "Id";
                // Восстанавливаем выбранное значение
                if (selectedValue is not null)
                    cbxProductName.SelectedValue = selectedValue;
            }
            else
            {
                var errorMsg = response.Message ?? "Unknown error";
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
            Response<List<WarehouseStockResponse>> response;
            if (productId.HasValue && productId.Value != 0)
            {
                response = await warehouseItemsApi.GetProductDetailsFromWarehouseAsync(productId.Value).Handle();
            }
            else
            {
                // Если productId не задан, можно либо не загружать данные, либо загрузить все элементы склада
                // Здесь я выбрал не загружать ничего
                cbxPerRollCount.ItemsSource = null;
                return;
            }
            if (response.IsSuccess)
            {
                var warehouseItems = response.Data;
                cbxPerRollCount.ItemsSource = warehouseItems;
                cbxPerRollCount.DisplayMemberPath = "LengthPerRoll";
                cbxPerRollCount.SelectedValuePath = "LengthPerRoll";
                // Восстанавливаем выбранное значение
                if (selectedValue is not null)
                    cbxPerRollCount.SelectedValue = selectedValue;
            }
            else
            {
                var errorMsg = response.Message ?? "Unknown error";
                MessageBox.Show("Ошибка при получении данных со склада: " + errorMsg);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Произошла ошибка: " + ex.Message);
        }
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        SaleItem saleItem = new()
        {
            CategoryId = cbxCategoryName.SelectedIndex,
            CategoryName = cbxCategoryName.Text,
            ProductId = (long)cbxProductName.SelectedValue,
            ProductName = cbxProductName.Text,
            PerRollCount = decimal.TryParse(cbxPerRollCount.Text, out decimal perRollCount) ? perRollCount : 0,
            RollCount = int.TryParse(txtRollCount.Text, out int rollCount) ? rollCount : 0,
            WarehouseCountRoll = sale.WarehouseCountRoll,
            Quantity = decimal.TryParse(txtQuantity.Text, out decimal quantity) ? quantity : 0,
            NewQuantity = sale.NewQuantity,
            WarehouseQuantity = sale.WarehouseQuantity,
            Price = decimal.TryParse(txtPrice.Text, out decimal price) ? price : 0,
            Sum = decimal.TryParse(txtSum.Text, out decimal sum) ? sum : 0,
            PerDiscount = decimal.TryParse(txtPerDiscount.Text, out decimal perDiscount) ? perDiscount : 0,
            Discount = decimal.TryParse(txtDiscount.Text, out decimal discount) ? discount : 0,
            FinalSumProduct = decimal.TryParse(txtFinalSumProduct.Text, out decimal finalSumProduct) ? finalSumProduct : 0
        };
        sale.SaleItems.Insert(0, saleItem);

        CalcSaleSum();
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
        sale.NewQuantity = 0;
        sale.WarehouseCountRoll = 0;
        sale.WarehouseQuantity = 0;
    }

    private void CalcSaleSum()
    {
        decimal finalSum = 0;
        decimal totalSum = 0;
        decimal totalDiscount = 0;
        if (sale.SaleItems.Count > 0)
        {
            sale.FinalSum = sale.SaleItems.Sum(s => s.Sum);
            sale.TotalSum = sale.SaleItems.Sum(s => s.Sum);
            sale.TotalDiscount = sale.SaleItems.Sum(d => d.Discount);
            finalSum = sale.FinalSum!.Value;
            totalSum = sale.TotalSum!.Value;
            totalDiscount = sale.TotalDiscount!.Value;
            if (sale.CheckedDiscount)
            {
                finalSum = totalSum - totalDiscount;
                sale.FinalSum = finalSum;
            }
        }
        decimal beginSum = decimal.TryParse(beginBalans.Text, out decimal value) ? value : 0;
        decimal endSum = beginSum - finalSum;
        lastBalans.Text = endSum.ToString();

    }

    private void CheckedDiscount_Click(object sender, RoutedEventArgs e)
    {
        CalcSaleSum();

    }

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        sale = new();
        ClearUI();
    }

    private async void SubmitButton_Click(object sender, RoutedEventArgs e)
    {
        if (saleDate.SelectedDate is null)
        {
            MessageBox.Show("Sana tanlanmagan!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            saleDate.Focus();
            return;
        }

        sale.OperationDate = saleDate.SelectedDate.Value;

        if (sale.SaleItems.Count == 0)
        {
            sale.Warning = "Hech qanday mahsulot kiritilmagan!";
            return;
        }


        var saleRequest = new SaleRequest
        {
            Date = sale.OperationDate,
            CustomerId = sale.CustomerId,
            Amount = sale.FinalSum ?? 0,
            Discount = sale.TotalDiscount ?? 0,
            Description = sale.Description,
            CurrencyId = sale.CurrencyId,
            Length = (decimal)sale.SaleItems.Sum(si => si.Quantity)!,
            RollCount = (int)sale.SaleItems.Sum(si => si.RollCount)!,
            IsApplied = sale.CheckedDiscount,
            Items = [.. sale.SaleItems.Select(i => new SaleItemRequest
            {
                ProductId = i.ProductId,
                RollCount = i.RollCount ?? 0,
                LengthPerRoll = i.PerRollCount ?? 0,
                TotalLength = i.Quantity ?? 0,
                UnitPrice = i.Price ?? 0,
                DiscountRate = i.PerDiscount ?? 0,
                DiscountAmount = i.Discount ?? 0,
                TotalAmount = i.FinalSumProduct ?? 0
            })]
        };

        var s = sale.CurrencyId;

        var response = await salesApi.CreateAsync(saleRequest)
            .Handle(isLoading => sale.IsLoading = isLoading);

        if (response.IsSuccess)
        {
            sale.Success = "Sotuv muvaffaqiyatli saqlandi!";
            //sale.SaleItems.Clear();
            sale = new();
            ClearUI();
            CustomerName.Focus();
        }
        else
        {
            sale.Error = $"Server xatosi: {response.StatusCode}";
        }
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        await LoadCategoryAsync();
        var response = await currenciesApi.GetAllAsync().Handle();
        if (response.IsSuccess)
        {
            CurrencyType.ItemsSource = response.Data;
            CurrencyType.SelectedValuePath = "Id";
            CurrencyType.SelectedItem = response.Data.FirstOrDefault(c => c.IsDefault);
        }
    }

    private void ClearUI()
    {
        // ComboBoxlarni tozalash
        CustomerName.Text = string.Empty;
        CustomerName.SelectedIndex = -1;

        cbxCategoryName.Text = string.Empty;
        cbxCategoryName.SelectedIndex = -1;

        cbxProductName.Text = string.Empty;
        cbxProductName.SelectedIndex = -1;

        cbxPerRollCount.Text = string.Empty;
        cbxPerRollCount.SelectedIndex = -1;

        //CurrencyType.Text = "So'm"; // hozircha faqat so'm bo'lgani uchun

        // TextBoxlarni tozalash
        txtRollCount.Text = string.Empty;
        txtQuantity.Text = string.Empty;
        txtPrice.Text = string.Empty;
        txtSum.Text = string.Empty;
        txtPerDiscount.Text = string.Empty;
        txtDiscount.Text = string.Empty;
        txtFinalSumProduct.Text = string.Empty;
        txtTotalDiscount.Text = string.Empty;
        finalSumm.Text = string.Empty;
        TotalSum.Text = string.Empty;
        noteTextBox.Text = string.Empty;
        beginBalans.Text = string.Empty;
        lastBalans.Text = string.Empty;
        tel.Text = string.Empty;

        // CheckBox va kalendarni tozalash
        checkedDiscount.IsChecked = false;
        saleDate.SelectedDate = DateTime.Now;

        sale.SaleItems.Clear();
        dataGrid.ItemsSource = sale.SaleItems;
    }

    private void supplyDate_LostFocus(object sender, RoutedEventArgs e)
    {
        // 1. Agar foydalanuvchi sanani kiritmagan bo‘lsa
        if (string.IsNullOrWhiteSpace(saleDate.dateTextBox.Text))
        {
            MessageBox.Show("Sana kiritilmagan!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            saleDate.Focus();
            return;
        }

        // 2. Qo‘lda yozilgan sanani DateTime ga o‘tkazamiz
        if (DateTime.TryParse(saleDate.dateTextBox.Text, out DateTime parsedDate))
        {
            saleDate.SelectedDate = parsedDate; // ✅ foydalanuvchi yozgan sana tanlangan bo‘ladi
        }
        else
        {
            MessageBox.Show("Kiritilgan sana noto‘g‘ri formatda!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            saleDate.Focus();
            return;
        }
    }

}