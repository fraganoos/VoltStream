namespace VoltStream.WPF.Payments.Views;

using VoltStream.WPF.Payments.ViewModels;
using ApiServices.Interfaces;
using ApiServices.Models.Responses;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

/// <summary>
/// Логика взаимодействия для PaymentsPage.xaml
/// </summary>
public partial class PaymentsPage : Page
{
    public Payment payment=new();
    private readonly ICustomersApi customersApi;
    private readonly ICurrenciesApi currenciesApi;

    public PaymentsPage(IServiceProvider service)
    {
        InitializeComponent();
        customersApi = service.GetRequiredService<ICustomersApi>();
        currenciesApi = service.GetRequiredService<ICurrenciesApi>();
        DataContext = payment;
        Loaded += PaymentsPage_Loaded;
        CustomerName.LostFocus += CustomerName_LostFocus;
    }

    private void CustomerName_LostFocus(object sender, RoutedEventArgs e)
    {
        if (CustomerName.SelectedValue is not null)
        {
            payment.CustomerId = (long)CustomerName.SelectedValue;
        }
        else
        {
            beginBalans.Clear();
            lastBalans.Text = null;
            //tel.Text = null;
            return;
        }
    }

    private async void PaymentsPage_Loaded(object sender, RoutedEventArgs e)
    {
        await LoadCustomerNameAsync();
        await LoadCurrenciesAsync();
    }

    private async Task LoadCustomerNameAsync()
    {
        try
        {
            // Сохраняем текущее выбранное значение
            var selectedValue = CustomerName.SelectedValue;
            var response = await customersApi.GetAllAsync();

            if (response.IsSuccess)
            {
                List<CustomerResponse> customers = response.Data!;
                CustomerName.ItemsSource = customers;
                CustomerName.DisplayMemberPath = "Name";
                CustomerName.SelectedValuePath = "Id";
                // Восстанавливаем выбранное значение
                if (selectedValue is not null)
                    CustomerName.SelectedValue = selectedValue;
            }
            else
            {
                // Проверяем на null, чтобы избежать CS8602
                var errorMsg = response.Message ?? "Unknown error";
                MessageBox.Show("Error fetching customers: " + errorMsg);
            }

        }
        catch (Exception ex)
        {
            MessageBox.Show("An error occurred: " + ex.Message);
        }
    }
    private async Task LoadCurrenciesAsync()
    {
        try
        {
            var selectedValue = CurrencyType.SelectedValue;
            var response = await currenciesApi.GetAllAsync();
            if (response.IsSuccess)
            {
                List<CurrencyResponse> currencies = response.Data!;
                CurrencyType.ItemsSource = currencies;
                CurrencyType.DisplayMemberPath = "Name";
                CurrencyType.SelectedValuePath = "Id";
                if (selectedValue is not null)
                    CurrencyType.SelectedValue = selectedValue;
            }
            else
            {
                var errorMsg = response.Message ?? "Unknown error";
                MessageBox.Show("Error fetching currencies: " + errorMsg);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("An error occurred: " + ex.Message);
        }
    }
}
