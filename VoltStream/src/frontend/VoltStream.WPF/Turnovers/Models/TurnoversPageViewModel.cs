using ApiServices.Enums;
using ApiServices.Extensions;
using ApiServices.Interfaces;
using ApiServices.Models.Responses;
using ClosedXML.Excel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DocumentFormat.OpenXml.Wordprocessing;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using WpfTextAlignment = System.Windows.TextAlignment;

using VoltStream.WPF.Commons;
using VoltStream.WPF.Commons.ViewModels;

namespace VoltStream.WPF.Turnovers.Models;

public partial class TurnoversPageViewModel : ViewModelBase
{
    private readonly ICustomersApi customersApi;
    private readonly ICustomerOperationsApi customerOperationsApi;
    private readonly IPaymentApi paymentApi;
    private readonly ISaleApi saleApi;
    private readonly IMapper mapper;

    private ObservableCollection<CustomerOperationForDisplayViewModel> allOperationsForDisplay = [];

    public TurnoversPageViewModel(IServiceProvider services)
    {
        customersApi = services.GetRequiredService<ICustomersApi>();
        customerOperationsApi = services.GetRequiredService<ICustomerOperationsApi>();
        paymentApi = services.GetRequiredService<IPaymentApi>();
        saleApi = services.GetRequiredService<ISaleApi>();
        mapper = services.GetRequiredService<IMapper>();

        _ = LoadInitialDataAsync();
    }

    [ObservableProperty] private CustomerResponse? selectedCustomer;
    [ObservableProperty] private ObservableCollection<CustomerResponse> customers = [];
    [ObservableProperty] private ObservableCollection<CustomerOperationViewModel> customerOperations = [];
    [ObservableProperty] private ObservableCollection<CustomerOperationForDisplayViewModel> customerOperationsForDisplay = [];
    [ObservableProperty] private DateTime? beginDate = DateTime.Now.AddMonths(-1);
    [ObservableProperty] private DateTime? endDate = DateTime.Now;

    private async Task LoadInitialDataAsync()
    {
        await LoadCustomersAsync();
        await LoadCustomerOperationsAsync();
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

    private async Task LoadCustomerOperationsAsync()
    {
        try
        {
            var response = await customerOperationsApi.GetAll().Handle(isLoading => IsLoading = isLoading);
            if (!response.IsSuccess || response.Data is null)
                return;

            CustomerOperations = mapper.Map<ObservableCollection<CustomerOperationViewModel>>(response.Data);
            await BuildDisplayListAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Operatsiyalar yuklanmadi: {ex.Message}");
        }
    }

    private async Task BuildDisplayListAsync()
    {
        var paymentsResponse = await paymentApi.GetAllAsync();
        var salesResponse = await saleApi.GetAll();

        var payments = paymentsResponse.Data ?? [];
        var sales = salesResponse.Data ?? [];

        var displayList = new ObservableCollection<CustomerOperationForDisplayViewModel>();

        foreach (var op in CustomerOperations)
        {
            DateTime date = DateTime.MinValue;

            if (op.OperationType == OperationType.Payment)
                date = payments.FirstOrDefault(p => p.CustomerOperation?.Id == op.Id)?.PaidAt.DateTime ?? DateTime.MinValue;
            else if (op.OperationType == OperationType.Sale)
                date = sales.FirstOrDefault(s => s.CustomerOperation?.Id == op.Id)?.Date.DateTime ?? DateTime.MinValue;

            var customer = Customers.FirstOrDefault(c => c.Id == op.Account.CustomerId);

            var display = new CustomerOperationForDisplayViewModel
            {
                Date = date,
                Customer = customer?.Name ?? "Noma’lum",
                Debit = op.OperationType == OperationType.Sale ? op.Amount : 0,
                Credit = op.OperationType == OperationType.Payment ? op.Amount : 0,
                Description = op.Description
            };

            displayList.Add(display);
        }

        allOperationsForDisplay = new ObservableCollection<CustomerOperationForDisplayViewModel>(displayList);
        ApplyFilter();
    }

    partial void OnSelectedCustomerChanged(CustomerResponse? value) => ApplyFilter();
    partial void OnBeginDateChanged(DateTime? value) => ApplyFilter();
    partial void OnEndDateChanged(DateTime? value) => ApplyFilter();

    private void ApplyFilter()
    {
        if (allOperationsForDisplay == null || allOperationsForDisplay.Count == 0)
            return;

        var filtered = allOperationsForDisplay.AsEnumerable();

        if (SelectedCustomer != null)
            filtered = filtered.Where(x => x.Customer == SelectedCustomer.Name);

        if (BeginDate.HasValue)
            filtered = filtered.Where(x => x.Date >= BeginDate.Value);

        if (EndDate.HasValue)
            filtered = filtered.Where(x => x.Date <= EndDate.Value);

        CustomerOperationsForDisplay = new ObservableCollection<CustomerOperationForDisplayViewModel>(
            filtered.OrderByDescending(x => x.Date)
        );
    }

    [RelayCommand]
    private void ClearFilter()
    {
        SelectedCustomer = null;
        BeginDate = DateTime.Now.AddMonths(-1);
        EndDate = DateTime.Now;
        CustomerOperationsForDisplay = new ObservableCollection<CustomerOperationForDisplayViewModel>(allOperationsForDisplay.OrderByDescending(x => x.Date));
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
                ws.Cell(1, 1).Value = "Mijoz operatsiyalari ro'yxati";
                ws.Range("A1:E1").Merge().Style
                    .Font.SetBold()
                    .Font.SetFontSize(16)
                    .Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

                string[] headers = { "Sana", "Mijoz", "Debit", "Kredit", "Izoh" };
                for (int i = 0; i < headers.Length; i++)
                    ws.Cell(3, i + 1).Value = headers[i];

                ws.Range("A3:E3").Style.Font.Bold = true;
                int row = 4;
                foreach (var item in CustomerOperationsForDisplay)
                {
                    ws.Cell(row, 1).Value = item.Date.ToString("dd.MM.yyyy");
                    ws.Cell(row, 2).Value = item.Customer;
                    ws.Cell(row, 3).Value = item.Debit;
                    ws.Cell(row, 4).Value = item.Credit;
                    ws.Cell(row, 5).Value = item.Description;
                    row++;
                }

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
    private void Preview()
    {
        if (CustomerOperationsForDisplay == null || !CustomerOperationsForDisplay.Any())
        {
            MessageBox.Show("Ko‘rsatish uchun ma’lumot yo‘q.", "Eslatma", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var doc = CreateFixedDocument();
        var previewWindow = new Window
        {
            Title = "Operatsiyalar Preview",
            Width = 900,
            Height = 800,
            Content = new DocumentViewer { Document = doc },
            WindowStartupLocation = WindowStartupLocation.CenterScreen
        };
        previewWindow.ShowDialog();
    }

    [RelayCommand]
    private void Print()
    {
        if (CustomerOperationsForDisplay == null || !CustomerOperationsForDisplay.Any())
        {
            MessageBox.Show("Chop etish uchun ma’lumot yo‘q.", "Eslatma", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var dlg = new PrintDialog();
        if (dlg.ShowDialog() == true)
            dlg.PrintDocument(CreateFixedDocument().DocumentPaginator, "Operatsiyalar");
    }

    private FixedDocument CreateFixedDocument()
    {
        var doc = new FixedDocument();
        var page = new FixedPage { Width = 793.7, Height = 1122.5, Background = Brushes.White };
        var stack = new StackPanel { Margin = new Thickness(40) };

        var title = new TextBlock
        {
            Text = "MIJOZ OPERATSIYALARI HISOBOTI",
            FontSize = 18,
            FontWeight = FontWeights.Bold,
            TextAlignment = System.Windows.TextAlignment.Center,
            Margin = new Thickness(0, 0, 0, 20)
        };
        stack.Children.Add(title);

        var grid = new Grid();
        string[] headers = { "Sana", "Mijoz", "Debit", "Kredit", "Izoh" };
        for (int i = 0; i < headers.Length; i++)
        {
            grid.ColumnDefinitions.Add(new ColumnDefinition());
            var txt = new TextBlock
            {
                Text = headers[i],
                FontWeight = FontWeights.Bold,
                TextAlignment = System.Windows.TextAlignment.Center,
                Padding = new Thickness(5)
            };
            Grid.SetColumn(txt, i);
            grid.Children.Add(txt);
        }

        stack.Children.Add(grid);

        foreach (var item in CustomerOperationsForDisplay)
        {
            var row = new Grid();
            for (int i = 0; i < 5; i++)
                row.ColumnDefinitions.Add(new ColumnDefinition());

            AddCell(row, 0, item.Date.ToString("dd.MM.yyyy"));
            AddCell(row, 1, item.Customer);
            AddCell(row, 2, item.Debit.ToString("N0"), System.Windows.TextAlignment.Right);
            AddCell(row, 3, item.Credit.ToString("N0"), System.Windows.TextAlignment.Right);
            AddCell(row, 4, item.Description);

            stack.Children.Add(row);
        }


        page.Children.Add(stack);
        var pc = new PageContent();
        ((IAddChild)pc).AddChild(page);
        doc.Pages.Add(pc);
        return doc;
    }

    private void AddCell(Grid grid, int column, string text, System.Windows.TextAlignment align = System.Windows.TextAlignment.Left)
    {
        var tb = new TextBlock
        {
            Text = text,
            TextAlignment = align,
            Padding = new Thickness(5)
        };
        Grid.SetColumn(tb, column);
        grid.Children.Add(tb);
    }

}
