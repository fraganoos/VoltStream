namespace VoltStream.WPF.Customer;

using System.Windows;

/// <summary>
/// Логика взаимодействия для CustomerWindow.xaml
/// </summary>
public partial class CustomerWindow : Window
{
    public CustomerWindow(string name = "")
    {
        InitializeComponent();
        txtName.Text = name;
        txtName.Focus();
    }
    public dynamic? Result { get; private set; }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        Result = new
        {
            name = txtName.Text,
            phone = txtPhone.Text,
            address = txtAddress.Text,
            description = txtDescription.Text,
            beginningSum = (decimal.TryParse(txtBeginningSum2.Text, out var d) ? d : 0) - (decimal.TryParse(txtBeginningSum.Text, out var k) ? k : 0),
            //discountSumm = decimal.TryParse(txtDiscountSum.Text, out var d) ? d : 0
        };

        DialogResult = true;
        Close();
    }
}
