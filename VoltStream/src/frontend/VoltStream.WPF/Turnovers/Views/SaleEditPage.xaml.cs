namespace VoltStream.WPF.Sales.Views;

using ApiServices.Models.Responses;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using VoltStream.WPF.Sales.ViewModels;

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