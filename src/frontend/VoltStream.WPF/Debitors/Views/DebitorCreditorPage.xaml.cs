namespace VoltStream.WPF.Debitors.Views;

using System.Windows.Controls;
using VoltStream.WPF.Debitors.Models;

/// <summary>
/// Interaction logic for DebitorCreditorPage.xaml
/// </summary>
public partial class DebitorCreditorPage : Page
{
    private readonly IServiceProvider service;
    public DebitorCreditorPage(IServiceProvider service)
    {
        InitializeComponent();
        this.service = service;
        DataContext = new DebitorCreditorPageViewModel(service);
    }
}
