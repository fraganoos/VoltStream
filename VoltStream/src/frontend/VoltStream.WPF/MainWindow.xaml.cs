namespace VoltStream.WPF;

using System.Windows;
using VoltStream.WPF.Commons.Services;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();

        DataContext = viewModel;

        NotificationService.Init(this);
    }
}
