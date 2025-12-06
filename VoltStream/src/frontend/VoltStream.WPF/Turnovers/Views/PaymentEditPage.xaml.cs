namespace VoltStream.WPF.Payments.Views;

using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using VoltStream.WPF.Payments.ViewModels;
using ApiServices.Models.Responses;

public partial class PaymentEditPage : Page
{
    private readonly PaymentEditViewModel viewModel;

    public PaymentEditPage(IServiceProvider services, PaymentResponse paymentData)
    {
        InitializeComponent();

        viewModel = ActivatorUtilities.CreateInstance<PaymentEditViewModel>(services, paymentData);
        DataContext = viewModel;

        // Window'ni yopish uchun event handler
        viewModel.CloseRequested += (s, e) =>
        {
            var window = Window.GetWindow(this);
            if (window != null)
            {
                window.DialogResult = e;
                window.Close();
            }
        };
    }
}