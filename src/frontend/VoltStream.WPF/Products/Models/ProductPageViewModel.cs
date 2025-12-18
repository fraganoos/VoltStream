namespace VoltStream.WPF.Products.Models;

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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using VoltStream.WPF.Commons;

public partial class ProductPageViewModel : ViewModelBase
{
    private readonly IServiceProvider services;

    public ProductPageViewModel(IServiceProvider services)
    {
        this.services = services;
        _ = LoadInitialDataAsync();
    }

    [ObservableProperty] private CategoryResponse? selectedCategory;
    [ObservableProperty] private ObservableCollection<CategoryResponse> categories = [];

    [ObservableProperty] private ProductResponse? selectedProduct;
    [ObservableProperty] private ObservableCollection<ProductResponse> allProducts = [];
    [ObservableProperty] private ObservableCollection<ProductResponse> products = [];

    [ObservableProperty] private ObservableCollection<ProductItemViewModel> productItems = [];
    [ObservableProperty] private ObservableCollection<ProductItemViewModel> filteredProductItems = [];

    [ObservableProperty] private decimal? finalAmount;

    private async Task LoadInitialDataAsync()
    {
        await Task.WhenAll(
            LoadCategoriesAsync(),
            LoadProductsAsync(),
            LoadWarehouseItemsAsync()
        );

        ApplyFilter();
    }

    [RelayCommand]
    private void ClearFilter()
    {
        SelectedCategory = null;
        SelectedProduct = null;

        Products = new ObservableCollection<ProductResponse>(AllProducts);

        FilteredProductItems = new ObservableCollection<ProductItemViewModel>(ProductItems);

        FinalAmount = FilteredProductItems.Sum(x => x.TotalAmount);
    }

    [RelayCommand]
    private void ExportToExcel()
    {
        try
        {
            if (FilteredProductItems == null || !FilteredProductItems.Any())
            {
                MessageBox.Show("Eksport qilish uchun ma'lumot topilmadi.",
                                "Eslatma", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel fayllari (*.xlsx)|*.xlsx",
                FileName = "Mahsulotlar.xlsx"
            };

            if (dialog.ShowDialog() != true)
                return;

            using (var workbook = new ClosedXML.Excel.XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Mahsulotlar");

                worksheet.Cell(1, 1).Value = "Mahsulot turi";
                worksheet.Cell(1, 2).Value = "Nomi";
                worksheet.Cell(1, 3).Value = "Rulon uzunligi";
                worksheet.Cell(1, 4).Value = "Rulon soni";
                worksheet.Cell(1, 5).Value = "Jami";
                worksheet.Cell(1, 6).Value = "O‘lchov birligi";
                worksheet.Cell(1, 7).Value = "Narxi";
                worksheet.Cell(1, 8).Value = "Umumiy summa";

                var headerRange = worksheet.Range("A1:H1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;

                int row = 2;
                foreach (var item in FilteredProductItems)
                {
                    worksheet.Cell(row, 1).Value = item.Category;
                    worksheet.Cell(row, 2).Value = item.Name;

                    worksheet.Cell(row, 3).Value = item.RollLength;
                    worksheet.Cell(row, 3).Value = (int)item.RollLength!;
                    worksheet.Cell(row, 3).Style.NumberFormat.Format = "#,##0";

                    worksheet.Cell(row, 4).Value = item.Quantity;
                    worksheet.Cell(row, 4).Value = (int)item.Quantity!;
                    worksheet.Cell(row, 4).Style.NumberFormat.Format = "#,##0";

                    worksheet.Cell(row, 5).Value = item.TotalCount;
                    worksheet.Cell(row, 5).Value = (int)item.TotalCount!;
                    worksheet.Cell(row, 5).Style.NumberFormat.Format = "#,##0";

                    worksheet.Cell(row, 6).Value = item.Unit;

                    worksheet.Cell(row, 7).Value = item.Price;
                    worksheet.Cell(row, 7).Value = (decimal)item.Price!;
                    worksheet.Cell(row, 7).Style.NumberFormat.Format = "#,##0.00";

                    worksheet.Cell(row, 8).Value = item.TotalAmount;
                    worksheet.Cell(row, 8).Value = (decimal)item.TotalAmount!;
                    worksheet.Cell(row, 8).Style.NumberFormat.Format = "#,##0.00";

                    worksheet.Range(row, 3, row, 8)
                        .Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Right;

                    worksheet.Cell(row, 6).Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                    row++;
                }

                worksheet.Cell(row, 1).Value = "Jami";
                worksheet.Cell(row, 1).Style.Font.Bold = true;
                worksheet.Cell(row, 8).Value = FinalAmount ?? 0;
                worksheet.Cell(row, 8).Style.Font.Bold = true;
                worksheet.Cell(row, 8).Value = (decimal)FinalAmount!;
                worksheet.Cell(row, 8).Style.NumberFormat.Format = "#,##0.00";
                worksheet.Cell(row, 8).Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Right;

                worksheet.Columns().AdjustToContents();

                double headerWidth = worksheet.Cell(1, 8).GetValue<string>().Length + 5;
                worksheet.Column(8).Width = headerWidth;

                workbook.SaveAs(dialog.FileName);
            }

            MessageBox.Show("Ma'lumotlar muvaffaqiyatli Excel faylga eksport qilindi ✅",
                            "Tayyor", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik yuz berdi: {ex.Message}",
                            "Xato", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void Preview()
    {
        if (FilteredProductItems == null || !FilteredProductItems.Any())
        {
            MessageBox.Show("Ko‘rsatish uchun ma’lumot yo‘q.", "Eslatma", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var fixedDoc = CreateFixedDocumentForPrint();

        var previewWindow = new Window
        {
            Title = "Print Preview",
            Width = 900,
            Height = 800,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            Content = new DocumentViewer
            {
                Document = fixedDoc,
                Margin = new Thickness(10)
            }
        };

        previewWindow.ShowDialog();
    }

    [RelayCommand]
    private void Print()
    {
        if (FilteredProductItems == null || !FilteredProductItems.Any())
        {
            MessageBox.Show("Chop etish uchun ma’lumot topilmadi.", "Eslatma", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var fixedDoc = CreateFixedDocumentForPrint();

        var dlg = new PrintDialog();
        if (dlg.ShowDialog() == true)
        {
            dlg.PrintDocument(fixedDoc.DocumentPaginator, "Mahsulotlar ro'yxati");
        }
    }

    private FixedDocument CreateFixedDocumentForPrint()
    {
        double pageWidth = 793.700787;
        double pageHeight = 1122.51969;
        double margin = 40;

        var items = FilteredProductItems?.ToList() ?? [];
        int rowsPerPage = 25;

        int totalPages = (int)Math.Ceiling((double)items.Count / rowsPerPage);
        if (totalPages == 0) totalPages = 1;

        var fixedDoc = new FixedDocument();
        fixedDoc.DocumentPaginator.PageSize = new Size(pageWidth, pageHeight);

        for (int p = 0; p < totalPages; p++)
        {
            var page = new FixedPage
            {
                Width = pageWidth,
                Height = pageHeight,
                Background = Brushes.White
            };

            var rootGrid = new Grid
            {
                Width = pageWidth - margin * 2,
                Height = pageHeight - margin * 2
            };
            FixedPage.SetLeft(rootGrid, margin);
            FixedPage.SetTop(rootGrid, margin);

            rootGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            rootGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            rootGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var headerText = new TextBlock
            {
                Text = "Mahsulotlar qoldig'i",
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 8),
                TextAlignment = TextAlignment.Left
            };
            Grid.SetRow(headerText, 0);
            rootGrid.Children.Add(headerText);

            var tableGrid = new Grid { ShowGridLines = false };
            Grid.SetRow(tableGrid, 1);

            tableGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.4, GridUnitType.Star) }); // CategoryResponse
            tableGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2.4, GridUnitType.Star) }); // Name
            tableGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.0, GridUnitType.Star) }); // RollLength
            tableGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.0, GridUnitType.Star) }); // Quantity
            tableGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.9, GridUnitType.Star) }); // TotalCount
            tableGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0.9, GridUnitType.Star) }); // Unit
            tableGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.0, GridUnitType.Star) }); // Price
            tableGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1.2, GridUnitType.Star) }); // TotalAmount

            tableGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            string[] headers = ["Mahsulot turi", "Nomi", "Rulon uzunligi", "Rulon soni", "Jami", "O‘lchov", "Narxi", "Umumiy summa"];
            for (int c = 0; c < headers.Length; c++)
            {
                var border = new Border
                {
                    BorderThickness = new Thickness(0.5),
                    BorderBrush = Brushes.Black,
                    Background = Brushes.LightGray,
                    Padding = new Thickness(4, 2, 4, 2)
                };
                var tb = new TextBlock
                {
                    Text = headers[c],
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 12
                };
                border.Child = tb;
                Grid.SetRow(border, 0);
                Grid.SetColumn(border, c);
                tableGrid.Children.Add(border);
            }

            var pageItems = items.Skip(p * rowsPerPage).Take(rowsPerPage).ToList();
            int startRowIndex = 1;
            foreach (var item in pageItems)
            {
                tableGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(24) });

                string[] values =
                [
                item.Category ?? string.Empty,
                item.Name ?? string.Empty,
                item.RollLength?.ToString("N0") ?? string.Empty,
                item.Quantity?.ToString("N0") ?? string.Empty,
                item.TotalCount?.ToString() ?? string.Empty,
                item.Unit ?? string.Empty,
                item.Price?.ToString("N2") ?? string.Empty,
                (item.TotalAmount ?? 0m).ToString("N2")
            ];

                for (int c = 0; c < values.Length; c++)
                {
                    var cellBorder = new Border
                    {
                        BorderThickness = new Thickness(0.5),
                        BorderBrush = Brushes.Gray,
                        Padding = new Thickness(4, 2, 4, 2)
                    };
                    var cellText = new TextBlock
                    {
                        Text = values[c],
                        FontSize = 11,
                        VerticalAlignment = VerticalAlignment.Center,
                        TextWrapping = TextWrapping.NoWrap
                    };

                    if (c >= 2 && c <= 4 || c == 6 || c == 7)
                        cellText.TextAlignment = TextAlignment.Right;
                    else if (c == 5)
                        cellText.TextAlignment = TextAlignment.Center;
                    else
                        cellText.TextAlignment = TextAlignment.Left;

                    cellBorder.Child = cellText;
                    Grid.SetRow(cellBorder, startRowIndex);
                    Grid.SetColumn(cellBorder, c);
                    tableGrid.Children.Add(cellBorder);
                }

                startRowIndex++;
            }

            if (p == totalPages - 1)
            {
                tableGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(26) });
                int totalRowIndex = tableGrid.RowDefinitions.Count - 1;

                var mergeBorder = new Border
                {
                    BorderThickness = new Thickness(0.5),
                    BorderBrush = Brushes.Black,
                    Padding = new Thickness(4, 2, 4, 2),
                    Background = Brushes.Transparent
                };
                var mergeText = new TextBlock
                {
                    Text = "Jami",
                    FontWeight = FontWeights.Bold,
                    FontSize = 12,
                    VerticalAlignment = VerticalAlignment.Center,
                    TextAlignment = TextAlignment.Left
                };
                mergeBorder.Child = mergeText;

                Grid.SetRow(mergeBorder, totalRowIndex);
                Grid.SetColumn(mergeBorder, 0);
                Grid.SetColumnSpan(mergeBorder, 7);
                tableGrid.Children.Add(mergeBorder);
                var totalBorder = new Border
                {
                    BorderThickness = new Thickness(0.5),
                    BorderBrush = Brushes.Black,
                    Padding = new Thickness(4, 2, 4, 2)
                };
                var totalText = new TextBlock
                {
                    Text = (FinalAmount ?? 0m).ToString("N2"),
                    FontWeight = FontWeights.Bold,
                    FontSize = 12,
                    VerticalAlignment = VerticalAlignment.Center,
                    TextAlignment = TextAlignment.Right
                };
                totalBorder.Child = totalText;
                Grid.SetRow(totalBorder, totalRowIndex);
                Grid.SetColumn(totalBorder, 7);
                tableGrid.Children.Add(totalBorder);
            }

            rootGrid.Children.Add(tableGrid);
            
            var footerPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            Grid.SetRow(footerPanel, 2);

            var pageNumberText = new TextBlock
            {
                Text = $"{p + 1}-bet / {totalPages}",
                FontSize = 11,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 6, 0, 0)
            };
            footerPanel.Children.Add(pageNumberText);
            rootGrid.Children.Add(footerPanel);

            page.Children.Add(rootGrid);

            var pageContent = new PageContent();
            ((IAddChild)pageContent).AddChild(page);
            fixedDoc.Pages.Add(pageContent);
        }

        return fixedDoc;
    }

    public async Task LoadWarehouseItemsAsync()
    {
        try
        {
            FilteringRequest request = new()
            {
                Filters = new()
                {
                    ["Product"] = ["include:Category"]
                }
            };

            var srvc = services.GetRequiredService<IWarehouseStocksApi>();
            var response = await srvc.Filter(request).Handle();

            if (response.IsSuccess)
            {
                ProductItems.Clear();
                foreach (var item in response.Data!)
                {
                    ProductItems.Add(new ProductItemViewModel
                    {
                        Category = item.Product.Category.Name,
                        Name = item.Product.Name,
                        RollLength = item.LengthPerRoll,
                        Quantity = item.RollCount,
                        Price = item.UnitPrice,
                        Unit = item.Product.Unit,
                        TotalCount = (int)item.TotalLength,
                    });
                }
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik: {ex.Message}");
        }
    }

    partial void OnSelectedCategoryChanged(CategoryResponse? value)
    {
        if (value != null)
        {
            var filteredProducts = AllProducts
                .Where(p => p.CategoryId == value.Id)
                .ToList();

            Products.Clear();
            foreach (var product in filteredProducts)
                Products.Add(product);

            if (SelectedProduct != null && SelectedProduct.CategoryId != value.Id)
                SelectedProduct = null;
        }
        else
        {
            Products.Clear();
            foreach (var product in AllProducts)
                Products.Add(product);
        }

        ApplyFilter();
    }

    partial void OnSelectedProductChanged(ProductResponse? value)
    {
        ApplyFilter();
    }

    public async Task LoadCategoriesAsync()
    {
        try
        {
            var response = await services.GetRequiredService<ICategoriesApi>().GetAllAsync().Handle();
            var mapper = services.GetRequiredService<IMapper>();
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
            var response = await services.GetRequiredService<IProductsApi>().GetAllAsync().Handle();
            var mapper = services.GetRequiredService<IMapper>();
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

    private void ApplyFilter()
    {
        IEnumerable<ProductItemViewModel> filtered = ProductItems;

        if (SelectedCategory != null)
            filtered = filtered.Where(x => x.Category == SelectedCategory.Name);

        if (SelectedProduct != null)
            filtered = filtered.Where(x => x.Name == SelectedProduct.Name);

        FilteredProductItems = new ObservableCollection<ProductItemViewModel>(filtered);
        FinalAmount = FilteredProductItems.Sum(x => x.TotalAmount);
    }

    private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ProductItemViewModel.TotalAmount))
            RecalculateTotals();
    }

    private void RecalculateTotals()
    {
        FinalAmount = ProductItems.Sum(x => x.TotalAmount);
    }
}