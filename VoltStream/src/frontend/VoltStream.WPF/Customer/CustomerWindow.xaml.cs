namespace VoltStream.WPF.Customer;

using System.Windows;
using System.Windows.Controls;

/// <summary>
    /// Логика взаимодействия для CustomerWindow.xaml
/// </summary>
public partial class CustomerWindow : Window
{
public CustomerWindow(string name = "")
{
InitializeComponent();
txtName.Text = name;
txtName.GotFocus += (s, e) => TextBox_SelectAll(s, e);
txtAddress.GotFocus += (s, e) => TextBox_SelectAll(s, e);
txtPhone.GotFocus += (s, e) => TextBox_SelectAll(s, e);
txtDescription.GotFocus += (s, e) => TextBox_SelectAll(s, e);
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
                private void TextBox_SelectAll(object sender, RoutedEventArgs e)
                {
                if (sender is TextBox textBox)
                {
                if (!textBox.IsReadOnly && !string.IsNullOrEmpty(textBox.Text))
                {
textBox.Dispatcher.BeginInvoke(new Action(() => textBox.SelectAll()), System.Windows.Threading.DispatcherPriority.Input);
                    }
                    }
                    }
                    }
