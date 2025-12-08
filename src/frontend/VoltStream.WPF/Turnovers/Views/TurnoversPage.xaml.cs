namespace VoltStream.WPF.Turnovers.Views;

using Microsoft.Extensions.DependencyInjection;
using System.Windows.Controls;
using VoltStream.WPF.Turnovers.Models;


/// <summary>
/// Interaction logic for TurnoversPage.xaml
/// </summary>
public partial class TurnoversPage : Page
{
    public TurnoversPage()
    {
        InitializeComponent();
        DataContext = App.Services!.GetRequiredService<TurnoversPageViewModel>();

        cbxCustomer.Focus();
    }
}
