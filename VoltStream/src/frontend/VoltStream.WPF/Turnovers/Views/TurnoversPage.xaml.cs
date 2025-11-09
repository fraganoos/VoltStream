namespace VoltStream.WPF.Turnovers.Views;
using System.Windows.Controls;
using VoltStream.WPF.Turnovers.Models;


/// <summary>
/// Interaction logic for TurnoversPage.xaml
/// </summary>
public partial class TurnoversPage : Page
{
    private readonly IServiceProvider serviceProvider;

    public TurnoversPage(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        this.serviceProvider = serviceProvider;
        DataContext = new TurnoversPageViewModel(serviceProvider);
    }
}
