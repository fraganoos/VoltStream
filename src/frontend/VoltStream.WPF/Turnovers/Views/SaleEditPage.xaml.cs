namespace VoltStream.WPF.Sales.Views;

using ApiServices.Models.Responses;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VoltStream.WPF.Sales.ViewModels;

public partial class SaleEditPage : Page
{
    private readonly SaleEditViewModel viewModel;

    public SaleEditPage(IServiceProvider services, SaleResponse saleData)
    {
        InitializeComponent();
        viewModel = ActivatorUtilities.CreateInstance<SaleEditViewModel>(services, saleData);
        DataContext = viewModel;

        viewModel.CloseRequested += (s, e) =>
        {
            var window = Window.GetWindow(this);
            if (window is not null)
            {
                window.DialogResult = e;
                window.Close();
            }
        };
    }

    private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (dataGrid.SelectedItem is SaleItemViewModel item)
        {
            viewModel.EditItemCommand.Execute(item);
        }
    }

    private void DataGrid_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Delete && dataGrid.SelectedItem is SaleItemViewModel item)
        {
            viewModel.DeleteItemCommand.Execute(item);
            e.Handled = true;
        }
    }
}