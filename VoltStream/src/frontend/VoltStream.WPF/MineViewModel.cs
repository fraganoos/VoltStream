namespace VoltStream.WPF;

using System.Windows.Input;
using VoltStream.WPF.Commons;
using VoltStream.WPF.Supplies.Views;


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
    // constructor
    public MineViewModel()
    {
        ShowSuppliesViewCommand = new ViewModelCommand(ExicuteShowSuppliesViewCommand);
    }
    public MineViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        ShowSuppliesViewCommand = new ViewModelCommand(ExicuteShowSuppliesViewCommand);
    }

    private void ExicuteShowSuppliesViewCommand(object obj)
    {
        var view = new SuppliesPage();
        CurrentChildView = view;
    }
}
