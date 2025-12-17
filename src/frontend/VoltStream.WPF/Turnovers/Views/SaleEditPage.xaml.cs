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

        Loaded += Page_Loaded;
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        RegisterFocusNavigation();
    }

    private async void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (dataGrid.SelectedItem is SaleItemViewModel item)
        {
            await viewModel.EditItem(item);
        }
    }

    private void RegisterFocusNavigation()
    {
        FocusNavigator.RegisterElements(
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

        FocusNavigator.SetFocusRedirect(btnAdd, cbxCategory);
    }
}