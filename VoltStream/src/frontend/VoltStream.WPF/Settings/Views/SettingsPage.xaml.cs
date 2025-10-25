namespace VoltStream.WPF.Settings.Views;

using System.Windows.Controls;
using VoltStream.WPF.Settings.ViewModels;

/// <summary>
/// Interaction logic for SettingsPage.xaml
/// </summary>
public partial class SettingsPage : Page
{
    private readonly SettingsPageViewModel vm;
    public SettingsPage(SettingsPageViewModel vm)
    {
        InitializeComponent();
        this.vm = vm;
        DataContext = vm;
    }
}
