namespace VoltStream.WPF.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

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
