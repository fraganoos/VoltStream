namespace VoltStream.WPF.Payments.Views;

using ApiServices.Models.Responses;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using VoltStream.WPF.Payments.ViewModels;

public partial class PaymentEditPage : Page
{
    private readonly PaymentEditViewModel viewModel;

    public PaymentEditPage(IServiceProvider services, PaymentResponse paymentData)
    {
        InitializeComponent();

        viewModel = ActivatorUtilities.CreateInstance<PaymentEditViewModel>(services, paymentData);
        DataContext = viewModel;
    }
}