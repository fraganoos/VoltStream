namespace VoltStream.WPF.Supplies.Views;

using System.Windows;
using System.Windows.Controls;
using VoltStream.WPF.Commons.Services;
using VoltStream.WPF.Supplies.ViewModels;

public partial class SuppliesPage : Page
{
    public SuppliesPage(IServiceProvider services)
    {
        InitializeComponent();
        DataContext = new SuppliesPageViewModel(services);

        Loaded += Page_Loaded;
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        RegisterFocusNavigation();
    }

    private void RegisterFocusNavigation()
    {
        FocusNavigator.RegisterElements([
            date.TextBox,
            cbxCategory,
            cbxProduct,
            tbxPerRollCount,
            tbxRollCount,
            cbxUnit,
            txtUnitPrice,
            tbxDiscountRate,
            addSupplyBtn,
            cancelBtn
        ]);

        FocusNavigator.SetFocusRedirect(addSupplyBtn, cbxCategory);
        FocusNavigator.SetFocusRedirect(cancelBtn, cbxCategory);
    }

    private void CbxCategory_LostFocus(object sender, RoutedEventArgs e)
    {
        if (DataContext is SuppliesPageViewModel vm && sender is ComboBox cbx)
            if (!vm.ConfirmCategoryText(cbx.Text))
                FocusNavigator.FocusElement(cbx);
    }

    private void CbxProduct_LostFocus(object sender, RoutedEventArgs e)
    {
        if (DataContext is SuppliesPageViewModel vm && sender is ComboBox cbx)
            if (!vm.ConfirmProductText(cbx.Text))
                FocusNavigator.FocusElement(cbx);
    }
}
