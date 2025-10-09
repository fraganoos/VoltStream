namespace VoltStream.WPF.Products.Views;

using System.Windows.Controls;


/// <summary>
/// Interaction logic for ProductsPage.xaml
/// </summary>
public partial class ProductsPage : Page
{
    private ProductViewModel vm;
    private readonly IServiceProvider services;
    public ProductsPage(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
        vm = new ProductViewModel(services);
        DataContext = vm;
    }

}
