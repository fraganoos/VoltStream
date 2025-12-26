namespace VoltStream.WPF.Sales_history.Views;

using System.Windows;
using System.Windows.Controls;
using VoltStream.WPF.Sales_history.Models;

public partial class SalesHistoryPage : Page
{
    private readonly IServiceProvider serviceProvider;
    private SalesHistoryPageViewModel vm;
    public SalesHistoryPage(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        this.serviceProvider = serviceProvider;
        vm = new SalesHistoryPageViewModel(serviceProvider);
        DataContext = vm;
    }

    private async void BeginDate_LostFocus(object sender, System.Windows.RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(beginDate.TextBox.Text))
        {
            beginDate.Focus();
            return;
        }

        if (DateTime.TryParse(beginDate.TextBox.Text, out DateTime parsedDate))
        {
            beginDate.SelectedDate = parsedDate;
        }
        else
        {
            MessageBox.Show("Kiritilgan sana noto‘g‘ri formatda!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            beginDate.Focus();
            return;
        }
        await vm.LoadSalesHistoryAsync();

    }

    private async void EndDate_LostFocus(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(endDate.TextBox.Text))
        {
            endDate.Focus();
            return;
        }

        if (DateTime.TryParse(endDate.TextBox.Text, out DateTime parsedDate))
        {
            endDate.SelectedDate = parsedDate;
        }
        else
        {
            MessageBox.Show("Kiritilgan sana noto‘g‘ri formatda!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            endDate.Focus();
            return;
        }
        await vm.LoadSalesHistoryAsync();
    }
}
