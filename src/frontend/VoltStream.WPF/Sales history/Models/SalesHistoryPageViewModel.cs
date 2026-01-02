namespace VoltStream.WPF.Sales_history.Models;

using ApiServices.Extensions;
using ApiServices.Interfaces;
using ApiServices.Models;
using ApiServices.Models.Responses;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DocumentFormat.OpenXml;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using VoltStream.WPF.Commons;

public partial class SalesHistoryPageViewModel : ViewModelBase
{
    private readonly IServiceProvider services;
    private readonly IMapper mapper;

    public ICollectionView FilteredSaleItemsView { get; }
    private ObservableCollection<ProductItemViewModel> allSaleItems = [];

    public SalesHistoryPageViewModel(IMapper mapper, IServiceProvider services)
    {
        this.services = services;
        this.mapper = mapper;
        FilteredSaleItemsView = CollectionViewSource.GetDefaultView(allSaleItems);
        FilteredSaleItemsView.Filter = FilterLogic;
        _ = LoadInitialDataAsync();
    }

    [ObservableProperty] private CustomerResponse? selectedCustomer;
    [ObservableProperty] private ObservableCollection<CustomerResponse> customers = [];

    [ObservableProperty] private CategoryResponse? selectedCategory;
    [ObservableProperty] private ObservableCollection<CategoryResponse> categories = [];

    [ObservableProperty] private ProductResponse? selectedProduct;
    [ObservableProperty] private ObservableCollection<ProductResponse> allProducts = [];
    [ObservableProperty] private ObservableCollection<ProductResponse> products = [];

    [ObservableProperty] private decimal? finalAmount;
    [ObservableProperty] private DateTime beginDate = DateTime.Today.AddDays(-7);
    [ObservableProperty] private DateTime endDate = DateTime.Today;


    #region Load Data

    private async Task LoadInitialDataAsync()
    {
        await Task.WhenAll(
            LoadCategoriesAsync(),
            LoadProductsAsync(),
            LoadCustomersAsync(),
            LoadSalesHistoryAsync()
        );
    }

    public async Task LoadCategoriesAsync()
    {
        try
        {
            var response = await services.GetRequiredService<ICategoriesApi>().GetAllAsync().Handle(isLoading => IsLoading = isLoading);
            if (response.IsSuccess)
                Categories = mapper.Map<ObservableCollection<CategoryResponse>>(response.Data!);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Kategoriya yuklanmadi: {ex.Message}");
        }
    }

    public async Task LoadProductsAsync()
    {
        try
        {
            FilteringRequest request = new()
            {
                Filters = new()
                {
                    ["Category"] = ["include"]
                }
            };

            var response = await services.GetRequiredService<IProductsApi>().Filter(request).Handle(isLoading => IsLoading = isLoading);
            if (response.IsSuccess)
            {
                AllProducts = mapper.Map<ObservableCollection<ProductResponse>>(response.Data!);

                Products.Clear();
                foreach (var product in AllProducts)
                    Products.Add(product);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Mahsulotlar yuklanmadi: {ex.Message}");
        }
    }

    public async Task LoadCustomersAsync()
    {
        try
        {
            var response = await services.GetRequiredService<ICustomersApi>().GetAllAsync().Handle();
            if (response.IsSuccess)
                Customers = mapper.Map<ObservableCollection<CustomerResponse>>(response.Data!);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Mahsulotlar yuklanmadi: {ex.Message}");
        }
    }

    public async Task LoadSalesHistoryAsync()
    {
        FilteringRequest request = new()
        {
            Filters = new()
            {
                ["Items"] = ["include:Product.Category"],
                ["Customer"] = ["include"],
                ["date"] = [$">={BeginDate:o}", $"<{EndDate.AddDays(1):o}"]
            }
        };

        var srvc = services.GetRequiredService<ISaleApi>();
        var response = await srvc.Filtering(request).Handle(isLoading => IsLoading = isLoading);

        if (!response.IsSuccess)
        {
            Error = response.Message ?? "Sotuv detallarini yuklashda xatolik!";
            return;
        }


        allSaleItems.Clear();
        foreach (var sale in response.Data!)
        {
            foreach (var item in sale.Items)
            {
                allSaleItems.Add(new ProductItemViewModel
                {
                    OperationDate = sale.Date.LocalDateTime,
                    Category = item.Product!.Category.Name,
                    CategoryId = item.Product.CategoryId,
                    ProductId = item.Product.Id,
                    Name = item.Product.Name,
                    RollLength = item.LengthPerRoll,
                    Quantity = item.RollCount,
                    Price = item.UnitPrice,
                    Unit = item.Product.Unit,
                    TotalCount = (int)item.TotalLength,
                    Customer = sale.Customer?.Name,
                    CustomerId = sale.CustomerId
                });
            }
        }
        RefreshFilter();
    }

    #endregion Load Data

    #region Commands

    [RelayCommand]
    private async Task ClearFilter()
    {
        SelectedCategory = null;
        SelectedProduct = null;
        SelectedCustomer = null;
        await LoadSalesHistoryAsync();
    }
    [RelayCommand]
    private void ExportToExcel()
    {
        var visibleItems = FilteredSaleItemsView.Cast<ProductItemViewModel>().ToList();
        FinalAmount = visibleItems.Sum(x => x.TotalAmount);

        try
        {
            if (visibleItems == null || visibleItems.Count == 0)
            {
                MessageBox.Show("Eksport qilish uchun ma'lumot topilmadi.",
                                "Eslatma", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel fayllari (*.xlsx)|*.xlsx",
                FileName = "Savdo tarixi.xlsx"
            };

            if (dialog.ShowDialog() != true)
                return;

            using (var workbook = new ClosedXML.Excel.XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Savdo tarixi");

                string headerText = "Sotilgan mahsulotlar ro'yxati";


                worksheet.Cell(1, 1).Value = headerText;
                worksheet.Range("A1:J1").Merge();
                worksheet.Cell(1, 1).Style.Font.Bold = true;
                worksheet.Cell(1, 1).Style.Font.FontSize = 14;
                worksheet.Cell(1, 1).Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                worksheet.Row(1).Height = 25;

                worksheet.Cell(2, 1).Value = "Sana";
                worksheet.Cell(2, 2).Value = "Xaridor";
                worksheet.Cell(2, 3).Value = "Mahsulot turi";
                worksheet.Cell(2, 4).Value = "Nomi";
                worksheet.Cell(2, 5).Value = "Rulon uzunligi";
                worksheet.Cell(2, 6).Value = "Rulon soni";
                worksheet.Cell(2, 7).Value = "Jami";
                worksheet.Cell(2, 8).Value = "O‘lchov";
                worksheet.Cell(2, 9).Value = "Narxi";
                worksheet.Cell(2, 10).Value = "Umumiy summa";

                var headerRange = worksheet.Range("A2:J2");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;

                int row = 3;
                foreach (var item in visibleItems)
                {
                    worksheet.Cell(row, 1).Value = item.OperationDate?.ToString("dd.MM.yyyy");
                    worksheet.Cell(row, 2).Value = item.Customer ?? "";
                    worksheet.Cell(row, 3).Value = item.Category ?? "";
                    worksheet.Cell(row, 4).Value = item.Name ?? "";
                    worksheet.Cell(row, 5).Value = (int?)item.RollLength ?? 0;
                    worksheet.Cell(row, 5).Style.NumberFormat.Format = "#,##0";
                    worksheet.Cell(row, 6).Value = (int?)item.Quantity ?? 0;
                    worksheet.Cell(row, 6).Style.NumberFormat.Format = "#,##0";
                    worksheet.Cell(row, 7).Value = (int?)item.TotalCount ?? 0;
                    worksheet.Cell(row, 7).Style.NumberFormat.Format = "#,##0";
                    worksheet.Cell(row, 8).Value = item.Unit ?? "";
                    worksheet.Cell(row, 9).Value = (decimal?)item.Price ?? 0;
                    worksheet.Cell(row, 9).Style.NumberFormat.Format = "#,##0.00";
                    worksheet.Cell(row, 10).Value = (decimal?)item.TotalAmount ?? 0;
                    worksheet.Cell(row, 10).Style.NumberFormat.Format = "#,##0.00";

                    row++;
                }

                worksheet.Cell(row, 1).Value = "Jami";
                worksheet.Range(row, 1, row, 9).Merge();
                worksheet.Cell(row, 1).Style.Font.Bold = true;
                worksheet.Cell(row, 10).Value = (decimal?)FinalAmount ?? 0;
                worksheet.Cell(row, 10).Style.Font.Bold = true;
                worksheet.Cell(row, 10).Style.NumberFormat.Format = "#,##0.00";

                worksheet.Columns().AdjustToContents();
                workbook.SaveAs(dialog.FileName);
            }

            MessageBox.Show("Ma'lumotlar muvaffaqiyatli Excel faylga eksport qilindi ✅",
                            "Tayyor", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik: {ex.Message}", "Xato", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void Print()
    {
        var visibleItems = FilteredSaleItemsView.Cast<ProductItemViewModel>().ToList();

        if (visibleItems == null || visibleItems.Count == 0)
        {
            MessageBox.Show("Chop etish uchun ma’lumot topilmadi.",
                            "Eslatma", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var fixedDoc = CreateFixedDocumentForPrint();
        var dlg = new PrintDialog();
        if (dlg.ShowDialog() == true)
            dlg.PrintDocument(fixedDoc.DocumentPaginator, "Savdo tarixi");
    }

    [RelayCommand]
    private void Preview()
    {
        var visibleItems = FilteredSaleItemsView.Cast<ProductItemViewModel>().ToList();

        if (visibleItems == null || !visibleItems.Any())
        {
            MessageBox.Show("Ko‘rsatish uchun ma’lumot yo‘q.", "Eslatma", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        FinalAmount = visibleItems.Sum(x => x.TotalAmount);
        var fixedDoc = CreateFixedDocumentForPrint();
        var previewWindow = new Window
        {
            Title = "Print Preview",
            Width = 1050,
            Height = 850,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            Content = new DocumentViewer { Document = fixedDoc, Margin = new Thickness(20) }
        };
        previewWindow.ShowDialog();
    }

    #endregion Commands

    #region Print Helpers

    private bool FilterLogic(object obj)
    {
        if (obj is not ProductItemViewModel item) return false;

        bool matchesCategory = SelectedCategory == null || item.CategoryId == SelectedCategory.Id;
        bool matchesProduct = SelectedProduct == null || item.ProductId == SelectedProduct.Id;
        bool matchesCustomer = SelectedCustomer == null || item.CustomerId == SelectedCustomer.Id;

        return matchesCategory && matchesProduct && matchesCustomer;
    }

    private void RefreshFilter()
    {
        FilteredSaleItemsView.Refresh();
        FinalAmount = allSaleItems.Cast<ProductItemViewModel>()
                                  .Where(x => FilterLogic(x))
                                  .Sum(x => x.TotalAmount);
    }

    private void UpdateProductList(CategoryResponse? category)
    {
        if (category != null)
        {
            var filtered = AllProducts.Where(p => p.CategoryId == category.Id).ToList();
            Products.Clear();
            foreach (var p in filtered) Products.Add(p);
        }
        else
        {
            Products.Clear();
            foreach (var p in AllProducts) Products.Add(p);
        }
    }

    private void RecalculateTotals()
    {
        FinalAmount = FilteredSaleItemsView.Cast<ProductItemViewModel>().ToList().Sum(x => x.TotalAmount);
    }

    private FixedDocument CreateFixedDocumentForPrint()
    {
        double pageWidth = 793.7; // A4 Width
        double pageHeight = 1122.5; // A4 Height
        double margin = 20;

        var fixedDoc = new FixedDocument();
        fixedDoc.DocumentPaginator.PageSize = new Size(pageWidth, pageHeight);

        int maxRowsPerPage = 45;
        int pageNumber = 0;

        var items = FilteredSaleItemsView.Cast<ProductItemViewModel>().ToList();
        int totalPages = (int)Math.Ceiling(items.Count / (double)maxRowsPerPage);
        int processedItems = 0;

        while (processedItems < items.Count)
        {
            pageNumber++;

            var page = new FixedPage { Width = pageWidth, Height = pageHeight, Background = Brushes.White };

            // Sarlavha faqat birinchi betda chiqishi uchun shart
            if (pageNumber == 1)
            {
                var title = new TextBlock
                {
                    Text = "Sotilgan mahsulotlar ro‘yxati",
                    FontSize = 16,
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center,
                    Width = pageWidth
                };
                FixedPage.SetTop(title, 10);
                page.Children.Add(title);
            }

            // Agar 1-bet bo'lsa jadval sal pastroqdan (45), qolgan betlarda yuqoriroqdan (20) boshlanadi
            double gridTopMargin = (pageNumber == 1) ? 45 : 20;
            var grid = new Grid { Margin = new Thickness(margin, gridTopMargin, margin, 40) };

            // Ustun kengliklari
            double[] columnWidths = { 60, 90, 80, 90, 60, 60, 50, 50, 80, 100 };

            var headers = new[]
            {
                "Sana","Mijoz","Mahsulot turi","Nomi","To'plamda","To'plam soni","Jami","O‘lchov","Narxi","Umumiy summa"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(columnWidths[i], GridUnitType.Pixel) });
            }

            int row = 0;
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            for (int i = 0; i < headers.Length; i++)
            {
                var border = new Border
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(0.5),
                    Background = Brushes.LightGray,
                    Padding = new Thickness(2)
                };
                var text = new TextBlock
                {
                    Text = headers[i],
                    FontWeight = FontWeights.Bold,
                    FontSize = 10,
                    TextAlignment = TextAlignment.Center,
                    TextWrapping = TextWrapping.Wrap
                };
                border.Child = text;
                Grid.SetRow(border, row);
                Grid.SetColumn(border, i);
                grid.Children.Add(border);
            }

            var pageItems = items.Skip(processedItems).Take(maxRowsPerPage).ToList();

            foreach (var item in pageItems)
            {
                row++;
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                string[] values =
                {
                    item.OperationDate?.ToString("dd.MM.yyyy") ?? "",
                    item.Customer ?? "",
                    item.Category ?? "",
                    item.Name ?? "",
                    item.RollLength?.ToString("N0") ?? "",
                    item.Quantity?.ToString("N0") ?? "",
                    item.TotalCount?.ToString("N0") ?? "",
                    item.Unit ?? "",
                    item.Price?.ToString("N2") ?? "",
                    item.TotalAmount?.ToString("N2") ?? ""
                };

                for (int i = 0; i < values.Length; i++)
                {
                    var border = new Border
                    {
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(0.5),
                        Padding = new Thickness(4)
                    };
                    var text = new TextBlock
                    {
                        Text = values[i],
                        FontSize = 9,
                        TextAlignment = (i >= 4 ? TextAlignment.Right : TextAlignment.Left),
                        TextWrapping = TextWrapping.Wrap
                    };
                    border.Child = text;
                    Grid.SetRow(border, row);
                    Grid.SetColumn(border, i);
                    grid.Children.Add(border);
                }
            }

            processedItems += pageItems.Count;

            if (processedItems >= items.Count)
            {
                row++;
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                var totalBorder = new Border
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(0.5),
                    Padding = new Thickness(4, 4, 10, 4)
                };

                var totalLabel = new TextBlock
                {
                    Text = "JAMI:",
                    FontWeight = FontWeights.Bold,
                    FontSize = 10,
                    TextAlignment = TextAlignment.Left
                };
                totalBorder.Child = totalLabel;

                Grid.SetRow(totalBorder, row);
                Grid.SetColumn(totalBorder, 0);
                Grid.SetColumnSpan(totalBorder, 9);
                grid.Children.Add(totalBorder);

                var valueBorder = new Border
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(0.5),
                    Padding = new Thickness(4)
                };

                var totalValue = new TextBlock
                {
                    Text = (FinalAmount ?? 0).ToString("N2"),
                    FontWeight = FontWeights.Bold,
                    FontSize = 10,
                    TextAlignment = TextAlignment.Right
                };
                valueBorder.Child = totalValue;

                Grid.SetRow(valueBorder, row);
                Grid.SetColumn(valueBorder, 9);
                grid.Children.Add(valueBorder);
            }

            var pageNumberText = new TextBlock
            {
                Text = $"{pageNumber}-bet / {totalPages}",
                FontSize = 10,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            FixedPage.SetBottom(pageNumberText, 15);
            FixedPage.SetRight(pageNumberText, 30);
            page.Children.Add(pageNumberText);

            FixedPage.SetLeft(grid, margin);
            FixedPage.SetTop(grid, gridTopMargin); // Dinamik top margin
            page.Children.Add(grid);

            var pageContent = new PageContent();
            ((IAddChild)pageContent).AddChild(page);
            fixedDoc.Pages.Add(pageContent);
        }

        return fixedDoc;
    }


    #endregion Print Helpers

    #region Property Changed Handlers

    partial void OnSelectedCategoryChanged(CategoryResponse? value)
    {
        UpdateProductList(value);
        RefreshFilter();
    }

    partial void OnSelectedProductChanged(ProductResponse? value) => RefreshFilter();
    partial void OnSelectedCustomerChanged(CustomerResponse? value) => RefreshFilter();

    private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ProductItemViewModel.TotalAmount))
            RecalculateTotals();
    }

    #endregion Property Changed Handlers
}
