namespace VoltStream.WPF;

using System.Windows.Input;
using VoltStream.WPF.Commons;
using VoltStream.WPF.Payments.Views;
using VoltStream.WPF.Sales.Views;
using VoltStream.WPF.Supplies.Views;

public class MainViewModel : ViewModelBase
{
    private object currentChildView;
    private readonly IServiceProvider serviceProvider;
    public object CurrentChildView
    {
        get { return currentChildView; }
        set
        {
            currentChildView = value;
            OnPropertyChanged(nameof(CurrentChildView));
        }
    }

    // command to change view
    public ICommand ShowSuppliesViewCommand { get; }
    public ICommand ShowHomeViewCommand { get; }
    public ICommand ShowSalesViewCommand { get; }
    public ICommand ShowPaymentViewCommand { get; }

    // constructor
    public MainViewModel()
    {
        ShowSuppliesViewCommand = new ViewModelCommand(ExicuteShowSuppliesViewCommand);
        ShowSalesViewCommand = new ViewModelCommand(ExicuteShowSalesViewCommand);
        ShowPaymentViewCommand = new ViewModelCommand(ExicuteShowPaymentViewCommand);
    }

    public MainViewModel(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        ShowSuppliesViewCommand = new ViewModelCommand(ExicuteShowSuppliesViewCommand);
        ShowSalesViewCommand = new ViewModelCommand(ExicuteShowSalesViewCommand);
        ShowPaymentViewCommand = new ViewModelCommand(ExicuteShowPaymentViewCommand);
    }

    private void ExicuteShowSalesViewCommand(object obj)
    {
        var view = new SalesPage(serviceProvider);
        CurrentChildView = view;
    }

    private void ExicuteShowSuppliesViewCommand(object obj)
    {
        var view = new SuppliesPage(serviceProvider);
        CurrentChildView = view;
    }
    private void ExicuteShowPaymentViewCommand(object obj)
    {
        var view = new PaymentsPage(serviceProvider);
        CurrentChildView = view;
    }
}
