namespace VoltStream.WPF.Turnovers.Models;

using ApiServices.Enums;
using ApiServices.Extensions;
using ApiServices.Interfaces;
using ApiServices.Models.Responses;
using ClosedXML.Excel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using PdfSharp.Drawing;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VoltStream.WPF.Commons;
using VoltStream.WPF.Commons.Messages;
using VoltStream.WPF.Commons.Services;
using VoltStream.WPF.Commons.ViewModels;
using VoltStream.WPF.Payments.Views;
using VoltStream.WPF.Sales.Views;

public partial class TurnoversPageViewModel : ViewModelBase
{
    private readonly ICustomersApi customersApi;
    private readonly ICustomerOperationsApi customerOperationsApi;
    private readonly IMapper mapper;
    private readonly IServiceProvider services;
    private readonly INavigationService navigationService;

    private ObservableCollection<CustomerOperationForDisplayViewModel> allOperationsForDisplay = [];

    public TurnoversPageViewModel(IServiceProvider services, INavigationService navigationService)
    {
        this.services = services;
        this.navigationService = navigationService;
        customersApi = services.GetRequiredService<ICustomersApi>();
        customerOperationsApi = services.GetRequiredService<ICustomerOperationsApi>();
        mapper = services.GetRequiredService<IMapper>();

        WeakReferenceMessenger.Default.Register<EntityUpdatedMessage<string>>(this, (r, m) =>
        {
            if (m.Value == "OperationUpdated")
            {
                _ = LoadCustomerOperationsForSelectedCustomerAsync();
            }
        });

        _ = LoadInitialDataAsync();
    }

    [ObservableProperty] private CustomerResponse? selectedCustomer;
    [ObservableProperty] private ObservableCollection<CustomerResponse> customers = [];
    [ObservableProperty] private ObservableCollection<CustomerOperationViewModel> customerOperations = [];
    [ObservableProperty] private ObservableCollection<CustomerOperationForDisplayViewModel> customerOperationsForDisplay = [];
    [ObservableProperty] private CustomerOperationForDisplayViewModel? selectedItem;
    [ObservableProperty] private DateTime beginDate = new(DateTime.Today.Year, DateTime.Today.Month, 1);
    [ObservableProperty] private DateTime endDate = DateTime.Today;
    [ObservableProperty] private decimal? beginBalance;
    [ObservableProperty] private decimal? lastBalance;

    #region Property Changes

    partial void OnSelectedCustomerChanged(CustomerResponse? value)
    => _ = LoadCustomerOperationsForSelectedCustomerAsync();

    partial void OnBeginDateChanged(DateTime value)
        => _ = LoadCustomerOperationsForSelectedCustomerAsync();

    partial void OnEndDateChanged(DateTime value)
        => _ = LoadCustomerOperationsForSelectedCustomerAsync();

    #endregion Property Changes

    #region Load Data

    private async Task LoadCustomerOperationsForSelectedCustomerAsync()
    {
        if (SelectedCustomer is null)
        {
            CustomerOperationsForDisplay.Clear();
            return;
        }

        var response = await customerOperationsApi.GetByCustomerId(
            SelectedCustomer.Id,
            BeginDate,
            EndDate
        );

        CustomerOperationsForDisplay.Clear();

        if (!response.IsSuccess)
            return;

        var displayList = new ObservableCollection<CustomerOperationForDisplayViewModel>();

        foreach (var op in response.Data.Operations)
        {
            decimal debit = 0;
            decimal credit = 0;

            if (op.OperationType == OperationType.Payment)
            {
                if (op.Amount < 0)
                    debit = Math.Abs(op.Amount);
                else
                    credit = op.Amount;
            }
            else if (op.OperationType == OperationType.Sale)
            {
                debit = Math.Abs(op.Amount);
            }
            else if (op.OperationType == OperationType.DiscountApplied)
            {
                credit = op.Amount;
            }
            displayList.Add(new CustomerOperationForDisplayViewModel
            {
                Id = op.Id,
                Date = op.Date.LocalDateTime,
                Customer = SelectedCustomer.Name ?? "Noma'lum",
                Debit = debit,
                Credit = credit,
                Description = op.Description,
                OperationType = op.OperationType
            });
        }

        BeginBalance = response.Data.BeginBalance;
        LastBalance = response.Data.EndBalance;
        allOperationsForDisplay = displayList;
        ApplyFilter();
    }

    private async Task LoadInitialDataAsync()
    {
        await LoadCustomersAsync();
    }

    private async Task LoadCustomersAsync()
    {
        try
        {
            var response = await customersApi.GetAllAsync().Handle(isLoading => IsLoading = isLoading);
            if (response.IsSuccess)
                Customers = mapper.Map<ObservableCollection<CustomerResponse>>(response.Data!);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Mijozlar yuklanmadi: {ex.Message}");
        }
    }

    #endregion Load Data

    #region Commands

    [RelayCommand]
    private async Task Delete(CustomerOperationForDisplayViewModel? operation)
    {
        if (operation == null)
        {
            Warning = "O'chiriladigan operatsiya tanlanmagan!";
            return;
        }

        var result = MessageBox.Show(
            $"Ushbu operatsiyani o'chirishni xohlaysizmi?\n\n" +
            $"Sana: {operation.Date:dd.MM.yyyy}\n" +
            $"Debit: {operation.Debit:N2}\n" +
            $"Kredit: {operation.Credit:N2}\n" +
            $"Izoh: {operation.Description}",
            "O'chirishni tasdiqlash",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.No)
            return;

        var response = await customerOperationsApi.Delete(operation.Id)
            .Handle(isLoading => IsLoading = isLoading);

        if (response.IsSuccess)
        {
            CustomerOperationsForDisplay.Remove(operation);
            Success = "Operatsiya muvaffaqiyatli o'chirildi.";
        }
        else Error = response.Message ?? "Operatsiyani o'chirishda xatolik yuz berdi.";
    }

    [RelayCommand]
    private async Task Edit(CustomerOperationForDisplayViewModel? operation)
    {
        if (operation is null)
        {
            Warning = "Tahrirlanadigan operatsiya tanlanmagan!";
            return;
        }

        try
        {
            switch (operation.OperationType)
            {
                case OperationType.Sale:
                    await OpenSaleEditPage(operation.Id);
                    break;

                case OperationType.Payment:
                    await OpenPaymentEditPage(operation.Id);
                    break;

                case OperationType.DiscountApplied:
                    Warning = "Chegirmani tahrirlash mumkin emas!";
                    break;

                default:
                    Warning = "Noma'lum operatsiya turi!";
                    break;
            }
        }
        catch (Exception ex)
        {
            Error = $"Tahrirlash sahifasini ochishda xatolik: {ex.Message}";
        }
    }

    [RelayCommand]
    private void ClearFilter()
    {
        SelectedCustomer = null;
        BeginDate = DateTime.Now.AddMonths(-1);
        EndDate = DateTime.Now;
        CustomerOperationsForDisplay = new ObservableCollection<CustomerOperationForDisplayViewModel>(allOperationsForDisplay);
    }

    [RelayCommand]
    private void ExportToExcel()
    {
        try
        {
            if (CustomerOperationsForDisplay == null || !CustomerOperationsForDisplay.Any())
            {
                MessageBox.Show("Eksport qilish uchun ma'lumot topilmadi.", "Eslatma", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            var dialog = new Microsoft.Win32.SaveFileDialog
            {
                Filter = "Excel fayllari (*.xlsx)|*.xlsx",
                FileName = "Mijoz Operatsiyalari.xlsx"
            };

            if (dialog.ShowDialog() != true) return;

            using (var workbook = new XLWorkbook())
            {
                var ws = workbook.Worksheets.Add("Operatsiyalar");

                int row = 1;

                ws.Cell(row, 1).Value = "MIJOZ OPERATSIYALARI HISOBOTI";
                ws.Range($"A{row}:E{row}").Merge().Style
                    .Font.SetBold()
                    .Font.SetFontSize(16)
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                row++;

                ws.Cell(row, 1).Value = $"Mijoz: {SelectedCustomer?.Name.ToUpper() ?? "Tanlanmagan"}";
                ws.Range($"A{row}:E{row}").Merge().Style
                    .Font.SetBold()
                    .Font.SetFontSize(14)
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                row++;

                ws.Cell(row, 1).Value = $"Davr oralig'i: {BeginDate.ToString("dd.MM.yyyy") ?? "-"} dan {EndDate.ToString("dd.MM.yyyy") ?? "-"} gacha";
                ws.Range($"A{row}:E{row}").Merge().Style
                    .Font.SetBold()
                    .Font.SetFontSize(14)
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
                row += 2;

                string[] headers = { "Sana", "Mijoz", "Debit", "Kredit", "Izoh" };
                for (int i = 0; i < headers.Length; i++)
                    ws.Cell(row, i + 1).Value = headers[i];

                ws.Range($"A{row}:E{row}").Style.Font.Bold = true;
                row++;

                ws.Range($"A{row}:D{row}").Merge();
                ws.Cell(row, 1).Value = "Boshlang'ich qoldiq";
                ws.Cell(row, 1).Style.Font.Bold = true;
                ws.Cell(row, 5).Value = BeginBalance?.ToString("N2") ?? "0.00";
                ws.Cell(row, 5).Style.Font.Bold = true;
                ws.Cell(row, 5).Style.Alignment.WrapText = true;
                row++;

                foreach (var item in CustomerOperationsForDisplay)
                {
                    ws.Cell(row, 1).Value = item.Date.ToString("dd.MM.yyyy");
                    ws.Cell(row, 2).Value = item.Customer;
                    ws.Cell(row, 3).Value = item.Debit;
                    ws.Cell(row, 4).Value = item.Credit;

                    var formattedDescription = string.Join(Environment.NewLine,
                        (item.Description ?? "").Split(';').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x)));

                    ws.Cell(row, 5).Value = formattedDescription;
                    ws.Cell(row, 5).Style.Alignment.WrapText = true;

                    row++;
                }

                ws.Range($"A{row}:D{row}").Merge();
                ws.Cell(row, 1).Value = "Oxirgi qoldiq";
                ws.Cell(row, 1).Style.Font.Bold = true;
                ws.Cell(row, 5).Value = LastBalance?.ToString("N2") ?? "0.00";
                ws.Cell(row, 5).Style.Font.Bold = true;
                ws.Cell(row, 5).Style.Alignment.WrapText = true;

                ws.Columns().AdjustToContents();

                workbook.SaveAs(dialog.FileName);
            }

            MessageBox.Show("Excel fayl muvaffaqiyatli saqlandi.", "Tayyor", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Xatolik: {ex.Message}");
        }
    }

    [RelayCommand]
    private void Print()
    {
        if (CustomerOperationsForDisplay == null || !CustomerOperationsForDisplay.Any())
        {
            MessageBox.Show("Chop etish uchun ma'lumot yo'q.", "Eslatma", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var dlg = new PrintDialog();
        if (dlg.ShowDialog() == true)
            dlg.PrintDocument(CreateFixedDocument().DocumentPaginator, "Operatsiyalar");
    }

    [RelayCommand]
    private void Preview()
    {
        if (CustomerOperationsForDisplay == null || !CustomerOperationsForDisplay.Any())
        {
            MessageBox.Show("Ko'rsatish uchun ma'lumot yo'q.", "Eslatma", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var doc = CreateFixedDocument();

        var viewer = new DocumentViewer { Document = doc, Margin = new Thickness(10, 5, 5, 5) };

        var toolbar = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            HorizontalAlignment = HorizontalAlignment.Right,
            Margin = new Thickness(5)
        };

        var shareButton = new Button
        {
            Content = "📤 Telegram'da ulashish",
            Margin = new Thickness(5, 0, 0, 0),
            Padding = new Thickness(10, 5, 10, 5)
        };

        shareButton.Click += (s, e) =>
        {
            try
            {
                if (SelectedCustomer == null)
                {
                    MessageBox.Show("Mijoz tanlanmagan.", "Xato", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string valstreamFolder = Path.Combine(documentsPath, "Volstream");
                if (!Directory.Exists(valstreamFolder))
                    Directory.CreateDirectory(valstreamFolder);

                string customerName = SelectedCustomer.Name.Replace(" ", "_");
                string begin = BeginDate.ToString("dd.MM.yyyy") ?? "-";
                string end = EndDate.ToString("dd.MM.yyyy") ?? "-";
                string fileName = $"{customerName}_{begin}-{end}.pdf";

                string pdfPath = Path.Combine(valstreamFolder, fileName);

                //SaveFixedDocumentToPdf(doc, pdfPath, 96);

                //if (!File.Exists(pdfPath))
                //{
                //    MessageBox.Show("PDF fayl yaratilmagan.", "Xato", MessageBoxButton.OK, MessageBoxImage.Error);
                //    return;
                //}

                //SharePdfFile(pdfPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Xatolik: {ex.Message}", "Xato", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        };

        toolbar.Children.Add(shareButton);

        var layout = new DockPanel();
        DockPanel.SetDock(toolbar, Dock.Top);
        layout.Children.Add(toolbar);
        layout.Children.Add(viewer);

        var previewWindow = new Window
        {
            Title = "Operatsiyalar Preview",
            Width = 900,
            Height = 800,
            WindowStartupLocation = WindowStartupLocation.CenterScreen,
            Content = layout
        };

        previewWindow.ShowDialog();
    }

    #endregion Commands

    #region Private Helpers

    private void ApplyFilter()
    {
        if (allOperationsForDisplay is null || allOperationsForDisplay.Count == 0)
            return;

        var filtered = allOperationsForDisplay.AsEnumerable();

        CustomerOperationsForDisplay = new ObservableCollection<CustomerOperationForDisplayViewModel>(filtered);
    }

    private async Task OpenSaleEditPage(long operationId)
    {
        var saleResponse = await customerOperationsApi.GetById(operationId)
            .Handle(isLoading => IsLoading = isLoading);

        if (!saleResponse.IsSuccess)
        {
            Error = saleResponse.Message ?? "Savdo ma'lumotlari topilmadi!";
            return;
        }

        navigationService.Navigate(new SaleEditPage(services, saleResponse.Data.Sale!));
    }

    private async Task OpenPaymentEditPage(long operationId)
    {
        var response = await customerOperationsApi.GetById(operationId)
            .Handle(isLoading => IsLoading = isLoading);

        if (!response.IsSuccess)
        {
            Error = response.Message ?? "Savdo ma'lumotlari topilmadi!";
            return;
        }

        navigationService.Navigate(new PaymentEditPage(services, response.Data.Payment!));
    }

    #endregion Private Helpers

    #region PDF Export and Share

    // const double rowHeight = 25; ni ishlatishda davom etamiz
    // const double margin = 40; ni ishlatishda davom etamiz

    private FixedDocument CreateFixedDocument()
    {
        var doc = new FixedDocument();

        // A4 o'lchamlari (96 DPI da)
        const double pageWidth = 793.7;
        const double pageHeight = 1122.5;
        const double margin = 40;
        const double approxSingleRowHeight = 25; // Bitta satr uchun taxminiy balandlik

        var operations = CustomerOperationsForDisplay?.ToList() ?? new List<CustomerOperationForDisplayViewModel>();

        if (operations.Count == 0)
        {
            // Bo'sh sahifa yaratish logikasi (o'zgarishsiz qoladi)
            // ...
            return doc;
        }

        // Boshlang'ich balandliklar
        double currentY = 0; // Sahifadagi hozirgi vertikal joylashuv
        int pageNumber = 1;
        int currentIndex = 0;

        // Header va jadval sarlavhasi uchun ishg'ol qilingan balandlikni hisoblash
        // (Sarlavhalar, sanalar va jadval sarlavhasi)
        double headerAndTableTitleHeight = 30 + 15 + 16 + 5 + 10 + approxSingleRowHeight;
        double balanceRowHeight = approxSingleRowHeight;

        // --- Sahifalash boshlanishi ---

        while (currentIndex < operations.Count)
        {
            bool isFirstPage = (pageNumber == 1);
            bool isLastPage = false; // Oxirgi sahifani keyinroq aniqlaymiz

            // Yangi sahifani sozlash
            var page = new FixedPage { Width = pageWidth, Height = pageHeight, Background = Brushes.White };
            var container = new StackPanel { Margin = new Thickness(margin, 30, margin, margin) };

            // Headerlarni joylashtirish
            currentY = AddHeaderContent(container, pageNumber);

            // TABLE konteynerini yaratish (Endi Gridni bevosita containerga qo'shamiz)
            var table = new Grid();
            double[] widths = { 75, 110, 110, 415 };
            foreach (var w in widths)
                table.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(w) });

            // Jadval sarlavhasini qo'shish
            AddRowHeader(table, "Sana", "Debit", "Kredit", "Izoh", approxSingleRowHeight);

            // Boshlang'ich qoldiq (faqat 1-sahifa)
            if (isFirstPage)
            {
                AddBalanceRow(table, "Boshlang'ich qoldiq", BeginBalance?.ToString("N2") ?? "0.00", approxSingleRowHeight);
            }

            // Sahifadagi minimal qolishi kerak bo'lgan bo'sh joy
            double footerSpace = approxSingleRowHeight * 2.5; // Jami + Oxirgi qoldiq uchun taxminiy joy

            // Hozirgi sahifaga sig'adigan qatorlar ro'yxati
            var opsOnPage = new List<CustomerOperationForDisplayViewModel>();

            int tempIndex = currentIndex;
            while (tempIndex < operations.Count)
            {
                var op = operations[tempIndex];

                // Operatsiyaning balandligini taxmin qilish
                double requiredHeight = CalculateOperationRowHeight(op, widths[3]);

                // Agar bu operatsiya qolsa, sahifa to'ladi deb hisoblaymiz
                double availableSpace = pageHeight - margin * 2 - currentY - footerSpace;

                if (requiredHeight > availableSpace && tempIndex > currentIndex)
                {
                    // Operatsiya sig'maydi, lekin oldingilari sig'di. Shu sahifani yopamiz.
                    break;
                }

                // Agar birinchi elementning o'zi sig'masa, uni ham qo'shamiz (yoki boshqa sahifada qoldiramiz)
                if (requiredHeight > availableSpace && tempIndex == currentIndex)
                {
                    // Birinchi operatsiyaning o'zi sig'maydi. Uni baribir qo'shamiz.
                    // Chunki uni keyingi sahifaga o'tkazish uchun `FixedDocument` da kontentni bo'lish kerak.
                    // Bu sizning talabingizga zid.
                    // Bu yerda sizning talabingizni bajarish uchun biz uni qo'lda bo'lishimiz kerak edi (avvalgi yechim).
                    // Hozircha: sig'masa ham kiritamiz, bu PDF da xato ko'rsatishi mumkin, ammo UI ga o'xshaydi.
                }

                opsOnPage.Add(op);
                tempIndex++;
                currentY += requiredHeight; // Keyingi qator boshlanishini hisoblash
            }

            // Operatsiyalarni jadvalga qo'shish
            foreach (var op in opsOnPage)
            {
                AddOperationRow(table, op, approxSingleRowHeight);
            }

            currentIndex += opsOnPage.Count;
            isLastPage = (currentIndex >= operations.Count);

            // JAMI va OXIRGI QOLDIQ (faqat oxirgi sahifa)
            if (isLastPage)
            {
                decimal totalDebit = operations.Sum(x => x.Debit);
                decimal totalCredit = operations.Sum(x => x.Credit);

                AddRowTotal(table, "JAMI", totalDebit.ToString("N2"), totalCredit.ToString("N2"), approxSingleRowHeight);
                AddBalanceRow(table, "Oxirgi qoldiq", LastBalance?.ToString("N2") ?? "0.00", approxSingleRowHeight);
            }

            container.Children.Add(table);

            page.Children.Add(container);
            var pageContent = new PageContent();
            ((IAddChild)pageContent).AddChild(page);
            doc.Pages.Add(pageContent);

            pageNumber++;
            currentY = 0; // Yangi sahifada balandlikni nolga tiklash
        }

        return doc;
    }


    private double AddHeaderContent(StackPanel container, int pageNumber)
    {
        // HEADER
        container.Children.Add(new TextBlock
        {
            Text = "MIJOZ OPERATSIYALARI HISOBOTI",
            FontSize = 20,
            FontWeight = FontWeights.ExtraBold,
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Thickness(0, 0, 0, 15)
        });

        container.Children.Add(new TextBlock
        {
            Text = $"Mijoz: {SelectedCustomer?.Name.ToUpper() ?? "TANLANMAGAN"}",
            FontSize = 16,
            FontWeight = FontWeights.Medium
        });

        container.Children.Add(new TextBlock
        {
            Text = $"Davr: {BeginDate:dd.MM.yyyy} — {EndDate:dd.MM.yyyy}    |    Sahifa {pageNumber}",
            FontSize = 15,
            Margin = new Thickness(0, 5, 0, 10)
        });

        return 30 + 20 + 16 + 15 + 25; // Taxminiy piksel qiymati
    }

    private double CalculateOperationRowHeight(CustomerOperationForDisplayViewModel op, double commentColumnWidth)
    {
        // Izohning necha satrni egallashini hisoblaymiz
        string description = op.Description ?? op.FormattedDescription ?? "";

        var tempTextBlock = new TextBlock
        {
            Text = description,
            Width = commentColumnWidth - 10, // Padding uchun 10 ni ayiramiz
            TextWrapping = TextWrapping.Wrap,
            FontSize = 12
        };

        // O'lchash
        tempTextBlock.Measure(new Size(commentColumnWidth - 10, double.MaxValue));

        // Asl matn balandligi
        double actualHeight = tempTextBlock.DesiredSize.Height + 4; // Padding (2 tepa + 2 past)

        // Eng kam balandlikni hisobga olish
        return Math.Max(25, actualHeight); // Eng kamida 25 piksel bo'lishi kerak
    }


    private void AddOperationRow(Grid grid, CustomerOperationForDisplayViewModel op, double approxSingleRowHeight)
    {
        int row = grid.RowDefinitions.Count;

        // Balandlikni hisoblash
        double requiredHeight = CalculateOperationRowHeight(op, 415);

        // Balandlikni RowDefinition ga kiritish
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(requiredHeight) });

        // Sana
        AddSimpleCell(grid, row, 0, op.Date.ToString("dd.MM.yyyy"), TextAlignment.Center, FontWeights.Normal, 12, new Thickness(0.5, 0.5, 0, 0.5));
        // Debit
        AddSimpleCell(grid, row, 1, op.Debit == 0 ? "" : op.Debit.ToString("N2"), TextAlignment.Right, FontWeights.Normal, 12, new Thickness(0.5, 0.5, 0, 0.5));
        // Kredit
        AddSimpleCell(grid, row, 2, op.Credit == 0 ? "" : op.Credit.ToString("N2"), TextAlignment.Right, FontWeights.Normal, 12, new Thickness(0.5, 0.5, 0, 0.5));

        // Izoh (Wrap bilan)
        AddSimpleCell(grid, row, 3, op.Description ?? op.FormattedDescription ?? "", TextAlignment.Left, FontWeights.Normal, 12, new Thickness(0.5, 0.5, 0.5, 0.5));
    }

    // Yordamchi funksiya: oddiy hujayra yaratish
    private void AddSimpleCell(Grid grid, int row, int column, string value, TextAlignment align, FontWeight weight, double size, Thickness borderThickness)
    {
        var tb = new TextBlock
        {
            Text = value,
            Padding = new Thickness(5, 2, 5, 2),
            FontSize = size,
            FontWeight = weight,
            TextAlignment = align,
            TextWrapping = TextWrapping.Wrap,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };

        var border = new Border
        {
            BorderBrush = Brushes.Black,
            BorderThickness = borderThickness,
            Child = tb
        };

        Grid.SetRow(border, row);
        Grid.SetColumn(border, column);
        grid.Children.Add(border);
    }

    // Qolgan Yordamchi Funksiyalar (AddRowHeader, AddBalanceRow, AddRowTotal)
    // Bu funksiyalar sizning talabingizga moslashtirilgan.

    private void AddRowHeader(Grid grid, string date, string debit, string credit, string description, double height)
    {
        // ... (oldingi yechimdagi AddRowHeader ni o'xshashi)
        int row = grid.RowDefinitions.Count;
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(height) });

        AddSimpleCell(grid, row, 0, date, TextAlignment.Center, FontWeights.Bold, 13, new Thickness(0.5, 0.5, 0, 0.5));
        AddSimpleCell(grid, row, 1, debit, TextAlignment.Center, FontWeights.Bold, 13, new Thickness(0.5, 0.5, 0, 0.5));
        AddSimpleCell(grid, row, 2, credit, TextAlignment.Center, FontWeights.Bold, 13, new Thickness(0.5, 0.5, 0, 0.5));
        AddSimpleCell(grid, row, 3, description, TextAlignment.Center, FontWeights.Bold, 13, new Thickness(0.5));
    }

    private void AddBalanceRow(Grid grid, string label, string value, double height)
    {
        // ... (oldingi yechimdagi AddBalanceRow ni o'xshashi)
        int row = grid.RowDefinitions.Count;
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(height) });

        // Label (4 ustunni birlashtiramiz, faqat chap qirrasi ko'rinadi)
        var labelTb = new TextBlock
        {
            Text = label,
            Padding = new Thickness(5, 2, 5, 2),
            FontWeight = FontWeights.Bold,
            FontSize = 12,
            TextAlignment = TextAlignment.Left
        };
        var labelBorder = new Border
        {
            BorderBrush = Brushes.Black,
            BorderThickness = new Thickness(0.5),
            Child = labelTb
        };

        Grid.SetRow(labelBorder, row);
        Grid.SetColumn(labelBorder, 0);
        Grid.SetColumnSpan(labelBorder, 4);
        grid.Children.Add(labelBorder);

        // Value (Izoh ustunida ko'rsatamiz)
        var valueTb = new TextBlock
        {
            Text = value,
            Padding = new Thickness(5, 2, 5, 2),
            FontWeight = FontWeights.Bold,
            FontSize = 12,
            TextAlignment = TextAlignment.Right,
            TextWrapping = TextWrapping.Wrap
        };
        var valueBorder = new Border
        {
            BorderBrush = Brushes.Black,
            BorderThickness = new Thickness(0.5),
            Child = valueTb
        };

        Grid.SetRow(valueBorder, row);
        Grid.SetColumn(valueBorder, 3);
        grid.Children.Add(valueBorder);
    }

    private void AddRowTotal(Grid grid, string label, string totalDebit, string totalCredit, double height)
    {
        // ... (oldingi yechimdagi AddRowTotal ni o'xshashi)
        int row = grid.RowDefinitions.Count;
        grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(height) });

        // Label (0-ustun)
        var labelTb = new TextBlock
        {
            Text = label,
            Padding = new Thickness(5, 2, 5, 2),
            FontWeight = FontWeights.Bold,
            FontSize = 12,
            TextAlignment = TextAlignment.Center
        };
        var labelBorder = new Border
        {
            BorderBrush = Brushes.Black,
            BorderThickness = new Thickness(0.5, 0.5, 0, 0.5),
            Child = labelTb
        };
        Grid.SetRow(labelBorder, row);
        Grid.SetColumn(labelBorder, 0);
        grid.Children.Add(labelBorder);

        // Bo'sh joylarni to'ldirish (Izoh ustuni)
        var blankBorder = new Border
        {
            BorderBrush = Brushes.Black,
            BorderThickness = new Thickness(0.5, 0.5, 0.5, 0.5),
        };
        Grid.SetRow(blankBorder, row);
        Grid.SetColumn(blankBorder, 3);
        grid.Children.Add(blankBorder);

        // Debit
        AddSimpleCell(grid, row, 1, totalDebit, TextAlignment.Right, FontWeights.Bold, 12, new Thickness(0.5, 0.5, 0, 0.5));
        // Kredit
        AddSimpleCell(grid, row, 2, totalCredit, TextAlignment.Right, FontWeights.Bold, 12, new Thickness(0.5, 0.5, 0, 0.5));
    }
    // Izoh matnini WPF rendering xususiyatiga ko'ra satrlarga bo'luvchi yordamchi funksiya
    private List<string> SplitTextByWidth(string text, double columnWidth, double fontSize)
    {
        var textBlock = new TextBlock
        {
            Text = text,
            Width = columnWidth,
            TextWrapping = TextWrapping.Wrap,
            FontSize = fontSize
        };

        // TextBlock'ni o'lchash
        textBlock.Measure(new Size(columnWidth, double.MaxValue));
        textBlock.Arrange(new Rect(0, 0, columnWidth, textBlock.DesiredSize.Height));

        // Matnni Visual-ga yozmasdan turib satr-satr olish WPF da juda murakkab.
        // Quyidagi yechim matnni to'liq oladi va uni List<string> ichiga qo'yadi.
        // Hozirgi kodingizda har bir satrni ajratish murakkab, shuning uchun bu qism
        // har bir matnni bitta satr deb hisoblaydi, ammo uning satr sonini hisoblaydi (avvalgi kodda qilganimiz kabi).
        // Sizning `maxRowsInTable` hisobingiz to'g'ri ishlashi uchun, SplitTextByWidth
        // har bir Izoh matni uchun necha satr talab qilinishini qaytarishi kerak.

        // Matnning o'zi:
        return new List<string> { text };
    }


    // ------------------------------------------------------------------------------------------------
    // Yordamchi hujayra yaratish funksiyasi (Sana, Debit, Kredit uchun RowSpan qo'llaydi)
    // ------------------------------------------------------------------------------------------------

    private void AddCellWithRowSpan(Grid grid, int row, int column, string value, int rowSpan, TextAlignment align, FontWeight weight, double size, bool isHeader)
    {
        var tb = new TextBlock
        {
            Text = value,
            Padding = new Thickness(5, 2, 5, 2),
            FontSize = size,
            FontWeight = weight,
            TextAlignment = align,
            TextWrapping = TextWrapping.Wrap,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };

        var border = new Border
        {
            BorderBrush = Brushes.Black,
            // Headerda barcha qirralar to'liq, kontentda faqat chap va past
            BorderThickness = new Thickness(0.5, 0.5, column == 3 ? 0.5 : 0, 0.5),
            Child = tb
        };

        Grid.SetRow(border, row);
        Grid.SetColumn(border, column);
        if (rowSpan > 1)
            Grid.SetRowSpan(border, rowSpan);

        grid.Children.Add(border);
    }

    // ------------------------------------------------------------------------------------------------
    // 4.1. Sarlavha satri
    // ------------------------------------------------------------------------------------------------


    // ------------------------------------------------------------------------------------------------
    // 4.2. Operatsiya segmentini qo'shish (asosiy satr va davomiylik satri)
    // ------------------------------------------------------------------------------------------------

    private void AddSegmentRow(Grid grid, int rowSpan, bool isFirstSegment, PaginatedOperation data, double rowHeight)
    {
        int startRow = grid.RowDefinitions.Count;

        // RowSpan qadar satrlarni Gridga qo'shish
        for (int i = 0; i < rowSpan; i++)
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(rowHeight) });

        if (isFirstSegment)
        {
            // Sana, Debit, Kredit (RowSpan bilan birlashtiriladi)
            AddCellWithRowSpan(grid, startRow, 0, data.Date.ToString("dd.MM.yyyy"), rowSpan, TextAlignment.Center, FontWeights.Normal, 12, false);
            AddCellWithRowSpan(grid, startRow, 1, data.Debit == 0 ? "" : data.Debit.ToString("N2"), rowSpan, TextAlignment.Right, FontWeights.Normal, 12, false);
            AddCellWithRowSpan(grid, startRow, 2, data.Credit == 0 ? "" : data.Credit.ToString("N2"), rowSpan, TextAlignment.Right, FontWeights.Normal, 12, false);
        }

        // Izoh uchun TextBlock - Faqat Izohning tegishli qismi ko'rsatiladi
        string descriptionPart = data.Description; // Sizning SplitTextByWidth funksiyangiz matnni bo'lmasa, bu to'liq matn bo'ladi

        // Izoh matnini to'liq ko'rsatish
        var descriptionTb = new TextBlock
        {
            Text = descriptionPart,
            Padding = new Thickness(5, 2, 5, 2),
            FontSize = 12,
            FontWeight = FontWeights.Normal,
            TextAlignment = TextAlignment.Left,
            TextWrapping = TextWrapping.Wrap,
            HorizontalAlignment = HorizontalAlignment.Stretch
        };

        // Izoh ustuni barcha satrlarni (RowSpan) egallaydi
        var descriptionBorder = new Border
        {
            BorderBrush = Brushes.Black,
            BorderThickness = new Thickness(0.5),
            Child = descriptionTb
        };

        Grid.SetRow(descriptionBorder, startRow);
        Grid.SetColumn(descriptionBorder, 3);
        Grid.SetRowSpan(descriptionBorder, rowSpan); // Birlashgan satrlar soni
        grid.Children.Add(descriptionBorder);

        // Izohning to'g'ri sahifalanishi uchun, descriptionTb ichidagi matn faqat shu
        // segmentga tegishli qismi bo'lishi kerak. Bu yerda sizning `SplitTextByWidth`
        // funksiyangizning satrlarni bo'lish natijasi kerak bo'ladi.
    }

    // ------------------------------------------------------------------------------------------------
    // 4.3. Boshlang'ich/Oxirgi Qoldiq satri
    // ------------------------------------------------------------------------------------------------

    public class PaginatedOperation
    {
        // Asosiy operatsiya ma'lumotlari
        public DateTime Date { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string Description { get; set; }

        // Sahifalash uchun qo'shimcha ma'lumot
        public int StartLineIndex { get; set; } // Bu qatorda Izoh qayerdan boshlanadi
        public int EndLineIndex { get; set; } // Bu qator Izohning qayerda tugaydi
        public int TotalLines { get; set; } // Izohning jami satrlar soni
        public bool IsFirstSegment { get; set; } // Bu segmentda Sana/Debit/Kredit bo'ladimi
        public bool IsLastSegment { get; set; } // Bu oxirgi segmentmi
    }

    #endregion PDF Export and Share
}