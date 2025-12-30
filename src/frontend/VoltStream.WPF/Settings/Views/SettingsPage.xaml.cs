namespace VoltStream.WPF.Settings.Views;

using System.Windows.Controls;
using VoltStream.WPF.Settings.ViewModels;

public partial class SettingsPage : Page
{
    public SettingsPage(SettingsPageViewModel vm)
    {
        InitializeComponent();
        DataContext = vm;
    }
}
