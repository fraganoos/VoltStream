namespace VoltStream.WPF.Supplies.Views;

using System;
using System.Windows.Controls;
using VoltStream.WPF.Commons.Services;
using VoltStream.WPF.Supplies.ViewModels;

public partial class SuppliesPage : Page
{
    public SuppliesPage(IServiceProvider services)
    {
        InitializeComponent();
        DataContext = new SuppliesPageViewModel(services);
    }

    private void CbxCategory_LostFocus(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is SuppliesPageViewModel vm && sender is ComboBox cbx)
            if (!vm.ConfirmCategoryText(cbx.Text))
                FocusNavigator.FocusElement(cbx);
    }

    private void CbxProduct_LostFocus(object sender, System.Windows.RoutedEventArgs e)
    {
        if (DataContext is SuppliesPageViewModel vm && sender is ComboBox cbx)
            if (!vm.ConfirmProductText(cbx.Text))
                FocusNavigator.FocusElement(cbx);
    }
}
