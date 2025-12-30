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
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using VoltStream.WPF.Commons;

public partial class SalesHistoryPageViewModel : ViewModelBase
{
    private readonly IServiceProvider services;

    public SalesHistoryPageViewModel(IServiceProvider services)
    {
        this.services = services;
        _ = LoadInitialDataAsync();
    }

    [ObservableProperty] private CustomerResponse? selectedCustomer;
    [ObservableProperty] private ObservableCollection<CustomerResponse> customers = [];

    [ObservableProperty] private CategoryResponse? selectedCategory;
    [ObservableProperty] private ObservableCollection<CategoryResponse> categories = [];

    [ObservableProperty] private ProductResponse? selectedProduct;
    [ObservableProperty] private ObservableCollection<ProductResponse> allProducts = [];
    [ObservableProperty] private ObservableCollection<ProductResponse> products = [];

    [ObservableProperty] private ObservableCollection<ProductItemViewModel> filteredSaleItems = [];

    [ObservableProperty] private decimal? finalAmount;
    [ObservableProperty] private DateTime beginDate = new(DateTime.Today.Year, DateTime.Today.Month, 1);
    [ObservableProperty] private DateTime endDate = DateTime.Today;

    private async Task LoadInitialDataAsync()
    {
        await Task.WhenAll(
            LoadCategoriesAsync(),
            LoadProductsAsync(),
            LoadCustomersAsync(),
            LoadSalesHistoryAsync()
        );
    }

    public async Task LoadCustomersAsync()
    {
        try
        {
            var response = await services.GetRequiredService<ICustomersApi>().GetAllAsync().Handle();
            var mapper = services.GetRequiredService<IMapper>();
            if (response.IsSuccess)
                Customers = mapper.Map<ObservableCollection<CustomerResponse>>(response.Data!);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Mahsulotlar yuklanmadi: {ex.Message}");
        }
    }


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
        FinalAmount = FilteredSaleItems.Sum(x => x.TotalAmount);

        try
        {
            if (FilteredSaleItems == null || !FilteredSaleItems.Any())
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
                foreach (var item in FilteredSaleItems)
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
        if (FilteredSaleItems == null || !FilteredSaleItems.Any())
        {
            MessageBox.Show("Chop etish uchun ma’lumot topilmadi.",
                            "Eslatma", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var fixedDoc = CreateFixedDocument();
        var dlg = new PrintDialog();
        if (dlg.ShowDialog() == true)
            dlg.PrintDocument(fixedDoc.DocumentPaginator, "Savdo tarixi");
    }

    [RelayCommand]
    private void Preview()
    {
        if (FilteredSaleItems == null || !FilteredSaleItems.Any())
        {
            MessageBox.Show("Ko‘rsatish uchun ma’lumot yo‘q.", "Eslatma", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        FinalAmount = FilteredSaleItems.Sum(x => x.TotalAmount);
        var fixedDoc = CreateFixedDocument();
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

    private FixedDocument CreateFixedDocument()
    {
        var doc = new FixedDocument();
        const double pageWidth = 793.7;
        const double pageHeight = 1122.5;
        const double margin = 30;
        const double bottomReservedSpace = 60; // Footer va ozgina bo'sh joy uchun
        var itemsList = FilteredSaleItems.ToList();
        if (!itemsList.Any()) return doc;

        int currentItemIndex = 0;
        int pageNumber = 1;
        bool totalAdded = false;

        while (currentItemIndex < itemsList.Count || !totalAdded)
        {
            var page = new FixedPage { Width = pageWidth, Height = pageHeight, Background = Brushes.White };

            // Header har sahifada (sizning PDF misolda shunday)
            double currentTop = 25;
            var header = CreateHeader(pageWidth, margin);
            FixedPage.SetTop(header, currentTop);
            FixedPage.SetLeft(header, margin);
            page.Children.Add(header);
            currentTop = 80; // Headerdan keyin jadval boshlanishi (rasmdagi kabi biroz bo'sh joy)

            // Jadval
            var grid = new Grid { Width = pageWidth - (margin * 2) };
            double[] widths = { 80, 140, 85, 90, 90, 80, 100, 130 }; // Rasmdagi ustun kengligiga yaqin
            foreach (var w in widths)
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(w) });

            // Jadval sarlavhasi
            AddRow(grid, true, "Mahsulot turi", "Nomi", "To'plamda", "To'plam soni", "Jami", "O‘lchov", "Narxi", "Umumiy summa");

            // Avval jadval sarlavhasini o'lchaymiz (header balandligi uchun)
            grid.Measure(new Size(grid.Width, double.PositiveInfinity));
            double usedHeight = grid.DesiredSize.Height;

            // Ma'lumot qatorlarini qo'shish
            while (currentItemIndex < itemsList.Count)
            {
                var item = itemsList[currentItemIndex];

                // Vaqtincha grid bilan qator balandligini oldindan hisoblaymiz
                var tempGrid = new Grid { Width = grid.Width };
                foreach (var col in grid.ColumnDefinitions)
                    tempGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = col.Width });

                AddRow(tempGrid, false,
                    "kabel", // Hammasi kabel (tortqi bundan mustasno, lekin ko'p hollarda kabel)
                    item.Name ?? "",
                    item.RollLength?.ToString("N0") ?? "",
                    item.Quantity?.ToString("N0") ?? "",
                    item.TotalCount?.ToString("N0") ?? "",
                    item.Unit ?? "",
                    item.Price?.ToString("N2") ?? "",
                    item.TotalAmount?.ToString("N2") ?? "");

                tempGrid.Measure(new Size(grid.Width, double.PositiveInfinity));
                double rowHeight = tempGrid.DesiredSize.Height;

                // Agar qo'shsak sahifadan chiqib ketadimi?
                if (currentTop + usedHeight + rowHeight > pageHeight - bottomReservedSpace)
                    break; // Joy yetarli emas — keyingi sahifaga o'tamiz

                // Joy bor — asosiy gridga qo'shamiz
                AddRow(grid, false,
                    "kabel",
                    item.Name ?? "",
                    item.RollLength?.ToString("N0") ?? "",
                    item.Quantity?.ToString("N0") ?? "",
                    item.TotalCount?.ToString("N0") ?? "",
                    item.Unit ?? "",
                    item.Price?.ToString("N2") ?? "",
                    item.TotalAmount?.ToString("N2") ?? "");

                usedHeight += rowHeight;
                currentItemIndex++;
            }

            // Oxirgi sahifada "Jami" qatorini qo'shish
            if (currentItemIndex == itemsList.Count && !totalAdded)
            {
                var tempGrid = new Grid { Width = grid.Width };
                foreach (var col in grid.ColumnDefinitions)
                    tempGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = col.Width });

                AddRow(tempGrid, true, "Jami", "", "", "", "", "", "", FinalAmount?.ToString("N2") ?? "0.00");

                tempGrid.Measure(new Size(grid.Width, double.PositiveInfinity));
                double totalRowHeight = tempGrid.DesiredSize.Height;

                if (currentTop + usedHeight + totalRowHeight <= pageHeight - bottomReservedSpace)
                {
                    AddRow(grid, true, "Jami", "", "", "", "", "", "", FinalAmount?.ToString("N2") ?? "0.00");
                    usedHeight += totalRowHeight;
                    totalAdded = true;
                }
                // Agar sig'masa — keyingi sahifada avtomatik qo'shiladi
            }

            // Jadvalni sahifaga joylashtirish
            FixedPage.SetTop(grid, currentTop);
            FixedPage.SetLeft(grid, margin);
            page.Children.Add(grid);

            // Footer (har doim pastki o'ng burchakda)
            var footer = new TextBlock
            {
                Text = $"{pageNumber}-bet / [total]",
                FontSize = 11,
                FontWeight = FontWeights.Bold,
                TextAlignment = TextAlignment.Right,
                Width = 200
            };
            FixedPage.SetTop(footer, pageHeight - 35);
            FixedPage.SetLeft(footer, pageWidth - margin - 200);
            page.Children.Add(footer);

            var pageContent = new PageContent();
            ((IAddChild)pageContent).AddChild(page);
            doc.Pages.Add(pageContent);

            pageNumber++;
        }

        UpdateFinalPageNumbers(doc);
        return doc;
    }

    private void AddRow(Grid grid, bool isHeader, params string[] values)
    {
        int row = grid.RowDefinitions.Count;
        grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

        for (int i = 0; i < values.Length; i++)
        {
            TextAlignment alignment = isHeader
                ? TextAlignment.Center
                : i switch
                {
                    0 => TextAlignment.Left,   // Mahsulot turi
                    1 => TextAlignment.Left,   // Nomi
                    5 => TextAlignment.Left,   // O‘lchov
                    _ => TextAlignment.Right   // Raqamlar o'ngda
                };

            var tb = new TextBlock
            {
                Text = values[i],
                Padding = new Thickness(6),
                FontSize = isHeader ? 13 : 12,
                FontWeight = isHeader ? FontWeights.Bold : FontWeights.Normal,
                TextAlignment = alignment,
                VerticalAlignment = VerticalAlignment.Center
            };

            var border = new Border
            {
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(isHeader ? 1 : 0.5),
                Background = isHeader ? Brushes.LightGray : Brushes.Transparent,
                Child = tb
            };

            Grid.SetRow(border, row);
            Grid.SetColumn(border, i);
            grid.Children.Add(border);
        }
    }

    private void UpdateFinalPageNumbers(FixedDocument doc)
    {
        int total = doc.Pages.Count;
        foreach (PageContent pc in doc.Pages)
        {
            var page = (FixedPage)pc.Child;
            foreach (var child in page.Children.OfType<TextBlock>())
            {
                if (child.Text.Contains("[total]"))
                    child.Text = child.Text.Replace("[total]", total.ToString());
            }
        }
    }

    private FrameworkElement CreateHeader(double pageWidth, double margin)
    {
        var panel = new StackPanel { Width = pageWidth - (margin * 2) };
        panel.Children.Add(new TextBlock
        {
            Text = "Mahsulotlar qoldig‘i",
            FontSize = 22,
            FontWeight = FontWeights.ExtraBold,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 0, 0, 15)
        });
        return panel;
    }

    public async Task LoadSalesHistoryAsync()
    {
        try
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

            if (response.IsSuccess)
            {
                FilteredSaleItems.Clear();

                foreach (var sale in response.Data!)
                {
                    foreach (var item in sale.Items)
                    {
                        FilteredSaleItems.Add(new ProductItemViewModel
                        {
                            OperationDate = sale.Date.LocalDateTime,
                            Category = item.Product!.Category.Name,
                            Name = item.Product.Name,
                            RollLength = item.LengthPerRoll,
                            Quantity = item.RollCount,
                            Price = item.UnitPrice,
                            Unit = item.Product.Unit,
                            TotalCount = (int)item.TotalLength,
                            Customer = sale.Customer?.Name
                        });
                    }
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

    }

    partial void OnSelectedProductChanged(ProductResponse? value)
    {
        _ = LoadSalesHistoryAsync();
    }

    partial void OnSelectedCustomerChanged(CustomerResponse? value)
    {
        _ = LoadSalesHistoryAsync();
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
            FilteringRequest request = new()
            {
                Filters = new()
                {
                    ["Category"] = ["include"]
                }
            };

            var response = await services.GetRequiredService<IProductsApi>().Filter(request).Handle();
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

    private void Item_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(ProductItemViewModel.TotalAmount))
            RecalculateTotals();
    }

    private void RecalculateTotals()
    {
        FinalAmount = FilteredSaleItems.Sum(x => x.TotalAmount);
    }
}
