using System.Windows.Input;
using VoltStream.WPF.Commons;
using VoltStream.WPF.Sales.Views;
using VoltStream.WPF.Supplies.Views;

namespace VoltStream.WPF;

public class MineViewModel : ViewModelBase
{
    private object _currentChildView;
    private IServiceProvider _serviceProvider;
    public object CurrentChildView
    {
        get { return _currentChildView; }
        set
        {
            _currentChildView = value;
            OnPropertyChanged(nameof(CurrentChildView));
        }
    }
    // command to change view
    public ICommand ShowSuppliesViewCommand { get; }
    public ICommand ShowHomeViewCommand { get; }
    public ICommand ShowSalesViewCommand { get; }
    // constructor
    public MineViewModel()
    {
        ShowSuppliesViewCommand = new ViewModelCommand(ExicuteShowSuppliesViewCommand);
        ShowSalesViewCommand = new ViewModelCommand(ExicuteShowSalesViewCommand);
    }
    public MineViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        ShowSuppliesViewCommand = new ViewModelCommand(ExicuteShowSuppliesViewCommand);
        ShowSalesViewCommand = new ViewModelCommand(ExicuteShowSalesViewCommand);
    }

    private void ExicuteShowSalesViewCommand(object obj)
    {
        var view = new SalesPage(_serviceProvider);
        CurrentChildView = view;
    }

    private void ExicuteShowSuppliesViewCommand(object obj)
    {
        var view = new SuppliesPage(_serviceProvider);
        CurrentChildView = view;
    }
}
