namespace VoltStream.WPF.Payments.Views;

using ApiServices.Extensions;
using ApiServices.Models.Requests;
using CommunityToolkit.Mvvm.Messaging;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VoltStream.WPF.Commons;
using VoltStream.WPF.Customer;
using VoltStream.WPF.Payments.ViewModels;
using VoltStream.WPF.Commons.Messages;
using VoltStream.WPF.Commons.ViewModels;
using System;

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
        Loaded += PaymentsPage_Loaded;
        Unloaded += PaymentsPage_Unloaded;
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
        if (CustomerName.Text is null or "")
        {
            lastBalans.Clear();
            beginBalans.Clear();
            Discount.Clear();
        }

    }

    #region Messenger for Focus
    private void PaymentsPage_Loaded(object sender, RoutedEventArgs e)
    {
        // Регистрируем мессенджер
        WeakReferenceMessenger.Default.Register<FocusRequestMessage>(this, OnFocusRequestMessage);
    }
    private void PaymentsPage_Unloaded(object sender, RoutedEventArgs e)
    {
        // Отписываемся при выгрузке страницы (во избежание утечек)
        WeakReferenceMessenger.Default.Unregister<FocusRequestMessage>(this);
    }

    private async void OnFocusRequestMessage(object recipient, FocusRequestMessage m)
    {
        if (m.Value == "Income" && Chiqim.IsEnabled)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                Chiqim.Focus();
            });
        }
        else if (m.Value == "Expense" && Kirim.IsEnabled)
        {
            await Dispatcher.InvokeAsync(() =>
            {
                Kirim.Focus();
            });
        }
        else if (m.Value == "Discription")
        {
            await Dispatcher.InvokeAsync(() =>
            {
                Discription.Focus();
            });
        }
    }
    #endregion Messenger for Focus}
}