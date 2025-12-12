namespace VoltStream.WPF.Sales.Views;

using ApiServices.Models.Responses;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VoltStream.WPF.Commons.Services;
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

        FocusNavigator.AttachEnterNavigation(
            [
                calendar.calendar,
                cbxCustomer,
                cbxCurrency,
                txtSaleTotalAmount,
                txtSaleTotalDiscount,
                chkIsDiscountApplied,
                txtSaleAmount,
                txtSaleDescription,
                cbxCategory,
                cbxProduct,
                cbxLengthPerRoll,
                txtRollCount,
                txtTotalLength,
                txtUnitPrice,
                txtTotalAmount,
                txtDiscountRate,
                txtDiscountAmount,
                txtFinalAmount,
                btnAdd,
                btnCancel
            ]);
        btnAdd.Click += AddButton_Click;
    }

    private void AddButton_Click(object sender, RoutedEventArgs e)
    {
        Dispatcher.BeginInvoke(new Action(() =>
        {
            cbxCategory.Focus();
        }), System.Windows.Threading.DispatcherPriority.Input);
    }

    private async void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (dataGrid.SelectedItem is SaleItemViewModel item)
        {
            await viewModel.EditItem(item);
        }
    }
}