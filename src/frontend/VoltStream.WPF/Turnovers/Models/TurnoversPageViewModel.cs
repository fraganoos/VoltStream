namespace VoltStream.WPF.Turnovers.Models;

using ApiServices.Enums;
using ApiServices.Extensions;
using ApiServices.Interfaces;
using ApiServices.Models;
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
    private readonly IPaymentApi paymentApi;
    private readonly ISaleApi saleApi;
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
        paymentApi = services.GetRequiredService<IPaymentApi>();
        saleApi = services.GetRequiredService<ISaleApi>();
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

        if (!response.IsSuccess)
        {
            CustomerOperationsForDisplay.Clear();
            return;
        }

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

        if (result != MessageBoxResult.Yes)
            return;

        try
        {
            Response<bool> response;

            switch (operation.OperationType)
            {
                case OperationType.Sale:
                    response = await saleApi.Delete(operation.Id)
                        .Handle(isLoading => IsLoading = isLoading);
                    break;

                case OperationType.Payment:
                    response = await paymentApi.DeleteAsync(operation.Id)
                        .Handle(isLoading => IsLoading = isLoading);
                    break;

                case OperationType.DiscountApplied:
                    Warning = "Chegirmani to'g'ridan-to'g'ri o'chirib bo'lmaydi!";
                    return;

                default:
                    Warning = "Noma'lum operatsiya turi!";
                    return;
            }

            if (response.IsSuccess)
            {
                Success = "Operatsiya muvaffaqiyatli o'chirildi!";
                await LoadCustomerOperationsForSelectedCustomerAsync();
            }
            else
            {
                Error = response.Message ?? "Operatsiyani o'chirishda xatolik!";
            }
        }
        catch (Exception ex)
        {
            Error = $"Xatolik: {ex.Message}";
        }
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

                SaveFixedDocumentToPdf(doc, pdfPath, 96);

                if (!File.Exists(pdfPath))
                {
                    MessageBox.Show("PDF fayl yaratilmagan.", "Xato", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                SharePdfFile(pdfPath);
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

    private void SaveFixedDocumentToPdf(FixedDocument fixedDoc, string pdfPath, int dpi)
    {
        try
        {
            if (File.Exists(pdfPath))
                File.Delete(pdfPath);

            using var document = new PdfSharp.Pdf.PdfDocument();

            foreach (var pageContent in fixedDoc.Pages)
            {
                var fixedPage = pageContent.GetPageRoot(false);
                if (fixedPage == null) continue;

                fixedPage.Measure(new Size(fixedPage.Width, fixedPage.Height));
                fixedPage.Arrange(new Rect(new Size(fixedPage.Width, fixedPage.Height)));
                fixedPage.UpdateLayout();

                double scale = dpi / 96.0;
                int pixelWidth = (int)(fixedPage.Width * scale);
                int pixelHeight = (int)(fixedPage.Height * scale);

                var bmp = new RenderTargetBitmap(pixelWidth, pixelHeight, dpi, dpi, PixelFormats.Pbgra32);

                var vb = new VisualBrush(fixedPage);
                var dv = new DrawingVisual();
                using (var dc = dv.RenderOpen())
                {
                    dc.PushTransform(new ScaleTransform(scale, scale));
                    dc.DrawRectangle(vb, null, new Rect(new Point(0, 0), new Size(fixedPage.Width, fixedPage.Height)));
                }

                bmp.Render(dv);

                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bmp));
                using var ms = new MemoryStream();
                encoder.Save(ms);
                ms.Position = 0;

                var pdfPage = document.AddPage();
                pdfPage.Width = XUnit.FromMillimeter(210);
                pdfPage.Height = XUnit.FromMillimeter(297);

                using var gfx = XGraphics.FromPdfPage(pdfPage);
                using var image = XImage.FromStream(ms);

                double imgWidthPoints = image.PixelWidth * (72.0 / dpi);
                double imgHeightPoints = image.PixelHeight * (72.0 / dpi);

                double xRatio = pdfPage.Width / imgWidthPoints;
                double yRatio = pdfPage.Height / imgHeightPoints;
                double ratio = Math.Min(xRatio, yRatio);

                double drawWidth = imgWidthPoints * ratio;
                double drawHeight = imgHeightPoints * ratio;
                double offsetX = (pdfPage.Width - drawWidth) / 2;
                double offsetY = (pdfPage.Height - drawHeight) / 2;

                gfx.DrawImage(image, offsetX, offsetY, drawWidth, drawHeight);
            }

            document.Save(pdfPath);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"PDF yaratishda xatolik: {ex.Message}", "Xato", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void SharePdfFile(string pdfPath)
    {
        try
        {
            if (!File.Exists(pdfPath))
            {
                MessageBox.Show("Fayl topilmadi.", "Xato", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Process.Start(new ProcessStartInfo
            {
                FileName = "explorer.exe",
                Arguments = $"/select,\"{pdfPath}\"",
                UseShellExecute = true
            });

            Process.Start(new ProcessStartInfo
            {
                FileName = "ms-contact-share:",
                Arguments = $"\"{pdfPath}\"",
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Ulashishda xatolik: {ex.Message}", "Xato", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private FixedDocument CreateFixedDocument()
    {
        var doc = new FixedDocument();
        var page = new FixedPage
        {
            Width = 793.7,
            Height = 1122.5,
            Background = Brushes.White
        };

        var stack = new StackPanel { Margin = new Thickness(40) };

        stack.Children.Add(new TextBlock
        {
            Text = "MIJOZ OPERATSIYALARI HISOBOTI",
            FontSize = 18,
            FontWeight = FontWeights.Bold,
            TextAlignment = TextAlignment.Center,
            Margin = new Thickness(0, 0, 0, 20)
        });

        stack.Children.Add(new TextBlock
        {
            Text = $"Mijoz: {SelectedCustomer?.Name.ToUpper() ?? "Tanlanmagan"}",
            FontSize = 16,
            FontWeight = FontWeights.Medium,
            Margin = new Thickness(0, 0, 0, 5)
        });

        stack.Children.Add(new TextBlock
        {
            Text = $"Davr oralig'i: {BeginDate.ToString("dd.MM.yyyy") ?? "-"} dan {EndDate.ToString("dd.MM.yyyy") ?? "-"} gacha",
            FontSize = 16,
            FontWeight = FontWeights.Medium,
            Margin = new Thickness(0, 0, 0, 20)
        });

        double[] colWidths = { 75, 110, 110, 415 };

        var headerGrid = CreateRow(colWidths, true, "Sana", "Debit", "Kredit", "Izoh");
        stack.Children.Add(headerGrid);

        var beginGrid = CreateBalanceRow(colWidths, "Boshlang'ich qoldiq", BeginBalance?.ToString("N2") ?? "0.00");
        stack.Children.Add(beginGrid);

        foreach (var item in CustomerOperationsForDisplay)
        {
            var row = CreateRow(colWidths, false,
                item.Date.ToString("dd.MM.yyyy"),
                item.Debit == 0 ? "" : item.Debit.ToString("N2"),
                item.Credit == 0 ? "" : item.Credit.ToString("N2"),
                item.FormattedDescription
            );
            stack.Children.Add(row);
        }

        var totalDebit = CustomerOperationsForDisplay.Sum(x => x.Debit);
        var totalCredit = CustomerOperationsForDisplay.Sum(x => x.Credit);

        var totalGrid = CreateRow(
            colWidths,
            true,
            "Jami",
            totalDebit.ToString("N2"),
            totalCredit.ToString("N2"),
            ""
        );

        stack.Children.Add(totalGrid);

        var lastGrid = CreateBalanceRow(colWidths, "Oxirgi qoldiq", LastBalance?.ToString("N2") ?? "0.00");
        stack.Children.Add(lastGrid);

        page.Children.Add(stack);
        var pc = new PageContent();
        ((IAddChild)pc).AddChild(page);
        doc.Pages.Add(pc);

        return doc;
    }

    private Grid CreateRow(double[] colWidths, bool isHeader, params string[] cells)
    {
        var grid = new Grid { HorizontalAlignment = HorizontalAlignment.Stretch };

        for (int i = 0; i < colWidths.Length; i++)
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(colWidths[i]) });

        for (int i = 0; i < cells.Length; i++)
        {
            var tb = new TextBlock
            {
                Text = cells[i],
                Padding = new Thickness(5, 2, 5, 2),
                TextAlignment = isHeader ? TextAlignment.Center : i switch
                {
                    0 => TextAlignment.Center,
                    1 or 2 => TextAlignment.Right,
                    _ => TextAlignment.Left
                },
                FontWeight = isHeader ? FontWeights.Bold : FontWeights.Normal,
                FontSize = isHeader ? 13 : 12,
                TextWrapping = TextWrapping.Wrap,
                HorizontalAlignment = isHeader ? HorizontalAlignment.Center : HorizontalAlignment.Stretch
            };

            var border = new Border
            {
                BorderBrush = Brushes.Black,
                BorderThickness = new Thickness(0.5, 0.5, i == cells.Length - 1 ? 0.5 : 0, 0.5),
                Child = tb
            };

            Grid.SetColumn(border, i);
            grid.Children.Add(border);
        }

        return grid;
    }

    private Grid CreateBalanceRow(double[] colWidths, string label, string value)
    {
        var grid = new Grid { HorizontalAlignment = HorizontalAlignment.Stretch };

        for (int i = 0; i < colWidths.Length; i++)
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(colWidths[i]) });

        var labelText = new TextBlock
        {
            Text = label,
            Padding = new Thickness(5, 2, 5, 2),
            FontWeight = FontWeights.Bold,
            FontSize = 12,
            Foreground = Brushes.Black,
            TextAlignment = TextAlignment.Left
        };

        var labelBorder = new Border
        {
            BorderBrush = Brushes.Black,
            BorderThickness = new Thickness(0.5),
            Child = labelText
        };

        Grid.SetColumn(labelBorder, 0);
        Grid.SetColumnSpan(labelBorder, 4);
        grid.Children.Add(labelBorder);

        var valueText = new TextBlock
        {
            Padding = new Thickness(5, 2, 5, 2),
            FontWeight = FontWeights.Bold,
            FontSize = 12,
            Foreground = Brushes.Black,
            TextAlignment = TextAlignment.Left,
            TextWrapping = TextWrapping.Wrap
        };

        if (!string.IsNullOrWhiteSpace(value))
        {
            var parts = value.Split(';')
                             .Select(x => x.Trim())
                             .Where(x => !string.IsNullOrEmpty(x))
                             .ToList();

            for (int i = 0; i < parts.Count; i++)
            {
                valueText.Inlines.Add(parts[i]);
                if (i < parts.Count - 1)
                    valueText.Inlines.Add(new LineBreak());
            }
        }

        var valueBorder = new Border
        {
            BorderBrush = Brushes.Black,
            BorderThickness = new Thickness(0.5),
            Child = valueText
        };

        Grid.SetColumn(valueBorder, 3);
        grid.Children.Add(valueBorder);

        return grid;
    }

    #endregion PDF Export and Share
}