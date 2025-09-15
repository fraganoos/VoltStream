using System.Windows.Controls;
using VoltStream.WPF.Sales.Models;

namespace VoltStream.WPF.Sales.Views
{
    /// <summary>
    /// Логика взаимодействия для SalesPage.xaml
    /// </summary>
    public partial class SalesPage : Page
    {
        public Sale _sale = new Sale();
        public SalesPage(IServiceProvider services)
        {
            InitializeComponent();
            DataContext = _sale;
        }
    }
}
