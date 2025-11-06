namespace VoltStream.WPF.Payments.PayDiscountWindow.Views;

using System.Windows;

/// <summary>
/// Логика взаимодействия для PayDiscountWindow.xaml
/// </summary>
public partial class PayDiscountWindow : Window
{
    public PayDiscountWindow(long id, string name, decimal bonus)
    {
        InitializeComponent();
        txtCustomer.Text = name;
        AmauntDiscount.Text = bonus.ToString("N2");
        DiscountSum.Focus();
    }
}
