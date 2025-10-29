namespace VoltStream.WPF.Debitors.Models;

using ApiServices.Extensions;
using ApiServices.Interfaces;
using ClosedXML.Excel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using VoltStream.WPF.Commons;
using VoltStream.WPF.Commons.ViewModels;

public partial class DebitorCreditorPageViewModel : ViewModelBase
{
    private readonly IServiceProvider service;
    private readonly ICustomersApi customersApi;
    private readonly IMapper mapper;

    public DebitorCreditorPageViewModel(IServiceProvider service)
    {
        this.service = service;
        customersApi = service.GetRequiredService<ICustomersApi>();
        _ = LoadDate();
        mapper = service.GetRequiredService<IMapper>();
    }

    [ObservableProperty] private ObservableCollection<CustomerViewModel> availableCustomers = [];
    [ObservableProperty] private ObservableCollection<DebitorCreditorItemViewModel> debitorCreditorItems = [];

    // Yangi property: filtrlangan debitor/kreditor
    [ObservableProperty]
    private ObservableCollection<DebitorCreditorItemViewModel> filteredDebitorCreditorItems = [];
    [ObservableProperty] private CustomerViewModel? selectedCustomer;
    // Umumiy balans
    [ObservableProperty] private decimal finalDebitor;
    [ObservableProperty] private decimal finalKreditor;
    [ObservableProperty] private decimal finalAmount;

    [ObservableProperty] private string? sign; 
    [ObservableProperty] private decimal amount;

    // Belgilar ro'yxati
    public List<string> Signs { get; } = new() { ">", ">=", "=", "<", "<=", "<>" };

    // LoadCustomers ichida filtrni ham to'ldir
    partial void OnDebitorCreditorItemsChanged(ObservableCollection<DebitorCreditorItemViewModel> value)
    {
        FilteredDebitorCreditorItems = new ObservableCollection<DebitorCreditorItemViewModel>(value);
        FinalAmount = value.Sum(x => x.TotalBalance);
    }

    // Qo'shimcha: balansni ko'rsatish uchun
    [ObservableProperty] private decimal totalBalance;


    partial void OnSignChanged(string? value) => ApplyFilter();
    partial void OnAmountChanged(decimal value) => ApplyFilter();

    partial void OnSelectedCustomerChanged(CustomerViewModel? value)
    {
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        if (DebitorCreditorItems == null) return;

        var filtered = DebitorCreditorItems.ToList();

        // 1. Mijoz bo'yicha filtr
        if (SelectedCustomer != null)
        {
            filtered = filtered.Where(x => x.Customer == SelectedCustomer.Name).ToList();
        }

        // 2. Belgilar bo'yicha filtr — faqat to'ldirilgan ustunni hisobga ol
        if (!string.IsNullOrEmpty(Sign) && Amount > 0)
        {
            filtered = Sign switch
            {
                ">" => filtered.Where(x => (x.Debitor > 0 && x.Debitor > Amount) ||
                                           (x.Creditor > 0 && x.Creditor > Amount)).ToList(),
                ">=" => filtered.Where(x => (x.Debitor > 0 && x.Debitor >= Amount) ||
                                           (x.Creditor > 0 && x.Creditor >= Amount)).ToList(),
                "=" => filtered.Where(x => x.Debitor == Amount || x.Creditor == Amount).ToList(),
                "<" => filtered.Where(x => (x.Debitor > 0 && x.Debitor < Amount) ||
                                           (x.Creditor > 0 && x.Creditor < Amount)).ToList(),
                "<=" => filtered.Where(x => (x.Debitor > 0 && x.Debitor <= Amount) ||
                                           (x.Creditor > 0 && x.Creditor <= Amount)).ToList(),
                "<>" => filtered.Where(x => x.Debitor != Amount && x.Creditor != Amount).ToList(),
                _ => filtered
            };
        }

        FilteredDebitorCreditorItems = new ObservableCollection<DebitorCreditorItemViewModel>(filtered);

        FinalDebitor = filtered.Sum(x => x.Debitor);
        FinalKreditor = filtered.Sum(x => x.Creditor);
        FinalAmount = filtered.Sum(x => x.TotalBalance)*(-1);
    }
    private async Task LoadDate()
    {
        await LoadCustomers();
    }

    private async Task LoadCustomers()
    {
        var response = await customersApi.GetAllAsync().Handle(isLoading => IsLoading = isLoading);
        if (response.IsSuccess)
        {
            AvailableCustomers = mapper.Map<ObservableCollection<CustomerViewModel>>(response.Data);

            var items = response.Data.Select(c =>
            {
                var totalBalance = c.Accounts.Sum(a => a.Balance);
                return new DebitorCreditorItemViewModel
                {
                    Customer = c.Name,
                    Debitor = totalBalance < 0 ? -totalBalance : 0,
                    Creditor = totalBalance > 0 ? totalBalance : 0,
                    TotalBalance = totalBalance
                };
            }).ToList();

            DebitorCreditorItems = new ObservableCollection<DebitorCreditorItemViewModel>(items);
            FilteredDebitorCreditorItems = new ObservableCollection<DebitorCreditorItemViewModel>(items);

            // Yig'indilar
            FinalDebitor = items.Sum(x => x.Debitor);
            FinalKreditor = items.Sum(x => x.Creditor);
            FinalAmount = -items.Sum(x => x.TotalBalance); // + va - ning yig'indisi
        }
        else
        {
            Error = response.Message ?? "Mijozlarni yuklashda xatolik!";
        }
    }

    [RelayCommand]
    private void ClearFilter()
    {
        SelectedCustomer = null;
        Sign = null;
        Amount = 0;
        ApplyFilter();
    }

    [RelayCommand]
    private void ExportToExcel()
    {
        try
        {
            if (FilteredDebitorCreditorItems == null || !FilteredDebitorCreditorItems.Any())
            {
                MessageBox.Show("Eksport qilish uchun ma'lumot topilmadi.", "Eslatma", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel fayllari (*.xlsx)|*.xlsx",
                FileName = "Debitor va Kreditorlar.xlsx"
            };

            if (dialog.ShowDialog() != true) return;

            using (var workbook = new ClosedXML.Excel.XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("DebitorKreditor");

                // Sarlavha
                worksheet.Cell(1, 1).Value = "DEBITOR VA KREDITORLAR HISOBOTI";
                worksheet.Range("A1:D1").Merge();
                worksheet.Cell(1, 1).Style.Font.Bold = true;
                worksheet.Cell(1, 1).Style.Font.FontSize = 16;
                worksheet.Cell(1, 1).Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;
                worksheet.Row(1).Height = 30;

                // Ustun nomlari
                worksheet.Cell(3, 1).Value = "Mijoz";
                worksheet.Cell(3, 2).Value = "Debitor";
                worksheet.Cell(3, 3).Value = "Kreditor";
                worksheet.Cell(3, 4).Value = "Umumiy balans";

                var headerRange = worksheet.Range("A3:D3");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.LightGray;
                headerRange.Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Center;

                int row = 4;
                foreach (var item in FilteredDebitorCreditorItems)
                {
                    worksheet.Cell(row, 1).Value = item.Customer;
                    worksheet.Cell(row, 2).Value = item.Debitor;
                    worksheet.Cell(row, 2).Style.NumberFormat.Format = "#,##0.00";
                    worksheet.Cell(row, 3).Value = item.Creditor;
                    worksheet.Cell(row, 3).Style.NumberFormat.Format = "#,##0.00";
                    worksheet.Cell(row, 4).Value = item.TotalBalance;
                    worksheet.Cell(row, 4).Style.NumberFormat.Format = "#,##0.00";
                    row++;
                }

                // JAMI QATORI — O'Z USTUNINING TAGIDA
                worksheet.Cell(row, 1).Value = "Jami:";
                worksheet.Cell(row, 1).Style.Font.Bold = true;
                worksheet.Cell(row, 1).Style.Alignment.Horizontal = ClosedXML.Excel.XLAlignmentHorizontalValues.Right;

                worksheet.Cell(row, 2).Value = FinalDebitor;
                worksheet.Cell(row, 2).Style.Font.Bold = true;
                worksheet.Cell(row, 2).Style.NumberFormat.Format = "#,##0.00";

                worksheet.Cell(row, 3).Value = FinalKreditor;
                worksheet.Cell(row, 3).Style.Font.Bold = true;
                worksheet.Cell(row, 3).Style.NumberFormat.Format = "#,##0.00";

                worksheet.Cell(row, 4).Value = FinalAmount;
                worksheet.Cell(row, 4).Style.Font.Bold = true;
                worksheet.Cell(row, 4).Style.NumberFormat.Format = "#,##0.00";
                worksheet.Cell(row, 4).Style.Font.FontColor = XLColor.DarkBlue;

                // Chiziq
                worksheet.Range(row, 1, row, 4).Style.Border.TopBorder = ClosedXML.Excel.XLBorderStyleValues.Thin;

                worksheet.Columns().AdjustToContents();
                workbook.SaveAs(dialog.FileName);
            }

            MessageBox.Show("Ma'lumotlar muvaffaqiyatli eksport qilindi", "Tayyor", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik: {ex.Message}", "Xato", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
    [RelayCommand]
    private void Preview()
    {
        if (FilteredDebitorCreditorItems == null || !FilteredDebitorCreditorItems.Any())
        {
            MessageBox.Show("Ko‘rsatish uchun ma’lumot yo‘q.", "Eslatma", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        FinalAmount = -FilteredDebitorCreditorItems.Sum(x => x.TotalBalance);

        var fixedDoc = CreateFixedDocumentForPrint();
        var previewWindow = new Window
        {
            Title = "Debitor/Kreditor Preview",
            Width = 900,
            Height = 800,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            Content = new DocumentViewer
            {
                Document = fixedDoc,
                Margin = new Thickness(10, 5, 5, 5)
            }
        };
        previewWindow.ShowDialog();
    }

    [RelayCommand]
    private void Print()
    {
        if (FilteredDebitorCreditorItems == null || !FilteredDebitorCreditorItems.Any())
        {
            MessageBox.Show("Chop etish uchun ma’lumot topilmadi.", "Eslatma", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var fixedDoc = CreateFixedDocumentForPrint();
        var dlg = new PrintDialog();
        if (dlg.ShowDialog() == true)
            dlg.PrintDocument(fixedDoc.DocumentPaginator, "Debitor va Kreditorlar");
    }

    private FixedDocument CreateFixedDocumentForPrint()
    {
        double pageWidth = 793.7;   // A4
        double pageHeight = 1122.5;
        double margin = 40;         // Chap va o'ngdan 40px

        var fixedDoc = new FixedDocument();
        fixedDoc.DocumentPaginator.PageSize = new Size(pageWidth, pageHeight);

        const int maxRowsPerPage = 50;
        int pageNumber = 0;
        var items = FilteredDebitorCreditorItems.ToList();
        int totalPages = (int)Math.Ceiling(items.Count / (double)maxRowsPerPage);
        int processedItems = 0;

        // Kenglikni hisoblaymiz (marginlarni hisobga olib)
        double availableWidth = pageWidth - (2 * margin);
        double col1Width = availableWidth * 0.40; // Mijoz — 40%
        double col2Width = availableWidth * 0.20; // Debitor — 20%
        double col3Width = availableWidth * 0.20; // Kreditor — 20%
        double col4Width = availableWidth * 0.20; // Umumiy — 20%

        while (processedItems < items.Count)
        {
            pageNumber++;
            var page = new FixedPage { Width = pageWidth, Height = pageHeight, Background = Brushes.White };
            var grid = new Grid { Margin = new Thickness(margin, 70, margin, 70) };

            // === COLUMN DEFINITION — KENGlik BO'YICHA TO'LIQ TO'LDIRILADI ===
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(col1Width) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(col2Width) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(col3Width) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(col4Width) });

            int row = 0;
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            // === HEADER ===
            var headers = new[] { "Mijoz", "Debitor", "Kreditor", "Umumiy balans" };
            for (int i = 0; i < headers.Length; i++)
            {
                var border = new Border
                {
                    BorderBrush = Brushes.Black,
                    BorderThickness = new Thickness(1),
                    Background = Brushes.LightGray,
                    Padding = new Thickness(6)
                };
                var text = new TextBlock
                {
                    Text = headers[i],
                    FontWeight = FontWeights.Bold,
                    FontSize = 14,
                    TextAlignment = TextAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                border.Child = text;
                Grid.SetRow(border, row);
                Grid.SetColumn(border, i);
                grid.Children.Add(border);
            }

            // === MA'LUMOTLAR ===
            var pageItems = items.Skip(processedItems).Take(maxRowsPerPage).ToList();
            foreach (var item in pageItems)
            {
                row++;
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                string[] values = [
                    item.Customer,
                item.Debitor > 0 ? item.Debitor.ToString("N2") : "",
                item.Creditor > 0 ? item.Creditor.ToString("N2") : "",
                item.TotalBalance.ToString("N2")
                ];

                for (int i = 0; i < values.Length; i++)
                {
                    var border = new Border
                    {
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(0.5),
                        Padding = new Thickness(5)
                    };
                    var text = new TextBlock
                    {
                        Text = values[i],
                        FontSize = 14,
                        TextAlignment = (i >= 1 ? TextAlignment.Right : TextAlignment.Left),
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    border.Child = text;
                    Grid.SetRow(border, row);
                    Grid.SetColumn(border, i);
                    grid.Children.Add(border);
                }
            }

            processedItems += pageItems.Count;

            // === JAMI — oxirgi sahifada ===
            if (processedItems >= items.Count)
            {
                row += 2;
                grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

                var totalLabel = new TextBlock
                {
                    Text = "Jami:",
                    FontWeight = FontWeights.Bold,
                    FontSize = 14,
                    TextAlignment = TextAlignment.Left,
                    Padding = new Thickness(5),
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetRow(totalLabel, row);
                Grid.SetColumn(totalLabel, 0);
                grid.Children.Add(totalLabel);

                AddTotalCell(grid, row, 1, FinalDebitor);
                AddTotalCell(grid, row, 2, FinalKreditor);
                AddTotalCell(grid, row, 3, FinalAmount, true);

                // Chiziq
                var line = new System.Windows.Shapes.Rectangle
                {
                    Stroke = Brushes.Black,
                    StrokeThickness = 1.5,
                    Margin = new Thickness(0, -3, 0, 0)
                };
                Grid.SetRow(line, row - 1);
                Grid.SetColumn(line, 0);
                Grid.SetColumnSpan(line, 4);
                grid.Children.Add(line);
            }

            // === SARLAVHA ===
            var title = new TextBlock
            {
                Text = "DEBITOR VA KREDITORLAR HISOBOTI",
                FontSize = 18,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 15, 0, 15)
            };
            FixedPage.SetTop(title, 15);
            FixedPage.SetLeft(title, 50);
            page.Children.Add(title);

            // === SAHIFA RAQAMI ===
            var pageNum = new TextBlock
            {
                Text = $"Sahifa {pageNumber} / {totalPages}",
                FontSize = 11,
                FontStyle = FontStyles.Italic,
                HorizontalAlignment = HorizontalAlignment.Right,
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(0, 0, 60, 20)
            };
            FixedPage.SetBottom(pageNum, 20);
            FixedPage.SetRight(pageNum, 50);
            page.Children.Add(pageNum);

            page.Children.Add(grid);

            var pageContent = new PageContent();
            ((IAddChild)pageContent).AddChild(page);
            fixedDoc.Pages.Add(pageContent);
        }

        return fixedDoc;
    }

    // Helper
    private void AddTotalCell(Grid grid, int row, int col, decimal value, bool isFinal = false)
    {
        var cell = new Border
        {
            BorderBrush = Brushes.Black,
            BorderThickness = new Thickness(0.5),
            Background = isFinal ? Brushes.AliceBlue : Brushes.White,
            Padding = new Thickness(5)
        };
        var text = new TextBlock
        {
            Text = value.ToString("N2"),
            FontWeight = FontWeights.Bold,
            FontSize = 12,
            TextAlignment = TextAlignment.Right,
            Foreground = isFinal ? Brushes.DarkBlue : Brushes.Black,
            VerticalAlignment = VerticalAlignment.Center
        };
        cell.Child = text;
        Grid.SetRow(cell, row);
        Grid.SetColumn(cell, col);
        grid.Children.Add(cell);
    }
}
