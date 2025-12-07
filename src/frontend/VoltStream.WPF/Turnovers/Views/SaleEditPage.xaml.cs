namespace VoltStream.WPF.Sales.Views;

using ApiServices.Models.Responses;
using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using System.Windows.Input;
using VoltStream.WPF.Sales.ViewModels;
using VoltStream.WPF.Turnovers.Models;

public partial class SaleEditPage : Page
{
    private readonly SaleEditViewModel viewModel;

    public SaleEditPage(IServiceProvider services, SaleResponse saleData)
    {
        InitializeComponent();
        viewModel = ActivatorUtilities.CreateInstance<SaleEditViewModel>(services, saleData);
        DataContext = viewModel;
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