namespace VoltStream.WPF.Sales.Views;

using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using VoltStream.WPF.Sales.ViewModels;
using ApiServices.Models.Responses;

public partial class SaleEditPage : Page
{
    private readonly SaleEditViewModel viewModel;

    public SaleEditPage(IServiceProvider services, SaleResponse saleData)
    {
        InitializeComponent();

        viewModel = ActivatorUtilities.CreateInstance<SaleEditViewModel>(services, saleData);
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