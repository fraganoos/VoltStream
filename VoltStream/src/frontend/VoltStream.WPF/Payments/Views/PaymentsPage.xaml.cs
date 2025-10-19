namespace VoltStream.WPF.Payments.Views;

using ApiServices.Extensions;
using ApiServices.Models.Requests;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VoltStream.WPF.Commons;
using VoltStream.WPF.Customer;
using VoltStream.WPF.Payments.ViewModels;

/// <summary>
/// Логика взаимодействия для PaymentsPage.xaml
/// </summary>
public partial class PaymentsPage : Page
{
    PaymentPageViewModel vm;
    public PaymentsPage(IServiceProvider services)
    {
        InitializeComponent();
        vm = new PaymentPageViewModel(services);
        DataContext = vm;
    }

    private async void CustomerName_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
    {

        bool accept = ComboBoxHelper.BeforeUpdate(sender, e, "Xaridor", true);
        if (accept)
        {
            var win = new CustomerWindow(CustomerName.Text);
            if (win.ShowDialog() == true)
            {
                var customer = win.Result;
                CustomerRequest newCustomer = new()
                {
                    Name = customer!.name,
                    Phone = customer.phone,
                    Address = customer.address,
                    Description = customer.description,
                    Accounts = [new()
                    {
                        OpeningBalance = customer.beginningSum,
                        Balance = customer.beginningSum,
                        Discount = 0,
                        CurrencyId = 1
                    }]
                };

                var response = await vm.customersApi.CreateAsync(newCustomer).Handle();
                if (response.IsSuccess)
                {
                    CustomerName.Text = newCustomer.Name;
                    await vm.LoadCustomersAsync();
                }
                else
                {
                    e.Handled = true;
                    MessageBox.Show($"Xatolik yuz berdi. {response.Message}", "Xatolik", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                // тут можете сохранить customer в БД или список
            }
            else { e.Handled = true; }
        }
    }
}