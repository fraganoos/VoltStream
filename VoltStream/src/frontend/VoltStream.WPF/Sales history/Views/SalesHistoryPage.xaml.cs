namespace VoltStream.WPF.Sales_history.Views;
using System.Windows;
using System.Windows.Controls;
using VoltStream.WPF.Sales_history.Models;

/// <summary>
/// Interaction logic for SalesHistoryPage.xaml
/// </summary>
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

    private void beginDate_LostFocus(object sender, System.Windows.RoutedEventArgs e)
    {
        // 1. Agar foydalanuvchi sanani kiritmagan bo‘lsa
        if (string.IsNullOrWhiteSpace(beginDate.dateTextBox.Text))
        {
            //MessageBox.Show("Sana kiritilmagan!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            beginDate.Focus();
            return;
        }

        // 2. Qo‘lda yozilgan sanani DateTime ga o‘tkazamiz
        if (DateTime.TryParse(beginDate.dateTextBox.Text, out DateTime parsedDate))
        {
            beginDate.SelectedDate = parsedDate; // ✅ foydalanuvchi yozgan sana tanlangan bo‘ladi
        }
        else
        {
            MessageBox.Show("Kiritilgan sana noto‘g‘ri formatda!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            beginDate.Focus();
            return;
        }

    }

    private async void endDate_LostFocus(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(endDate.dateTextBox.Text))
        {
            //MessageBox.Show("Sana kiritilmagan!", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
            endDate.Focus();
            return;
        }

        // 2. Qo‘lda yozilgan sanani DateTime ga o‘tkazamiz
        if (DateTime.TryParse(endDate.dateTextBox.Text, out DateTime parsedDate))
        {
            endDate.SelectedDate = parsedDate; // ✅ foydalanuvchi yozgan sana tanlangan bo‘ladi
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
