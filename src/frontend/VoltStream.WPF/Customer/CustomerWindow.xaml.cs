namespace VoltStream.WPF.Customer;

using System.Windows;
using System.Windows.Controls;
using VoltStream.WPF.Commons.Services;

public partial class CustomerWindow : Window
{
    public CustomerWindow(string name = "")
    {
        InitializeComponent();
        txtName.Text = name;

        Loaded += PageLoaded;
    }

    private void PageLoaded(object sender, RoutedEventArgs e)
    {
        txtName.GotFocus += TextBox_SelectAll;
        txtAddress.GotFocus += TextBox_SelectAll;
        txtPhone.GotFocus += TextBox_SelectAll;
        txtDescription.GotFocus += TextBox_SelectAll;
        FocusNavigator.FocusElement(txtName);
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
        };

        DialogResult = true;
        Close();
    }

    private void TextBox_SelectAll(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox textBox)
            if (!textBox.IsReadOnly && !string.IsNullOrEmpty(textBox.Text))
                FocusNavigator.FocusElement(textBox);
    }
}