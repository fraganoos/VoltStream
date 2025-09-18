namespace VoltStream.WPF;
using System.Windows;
using VoltStream.WPF.Commons.Services;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        NotificationService.Init(this);
    }
}