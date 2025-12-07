namespace VoltStream.WPF.Products.Views;

using System.Windows.Controls;
using VoltStream.WPF.Products.Models;


/// <summary>
/// Interaction logic for ProductsPage.xaml
/// </summary>
public partial class ProductsPage : Page
{
    private ProductPageViewModel vm;
    private readonly IServiceProvider services;
    public ProductsPage(IServiceProvider services)
    {
        InitializeComponent();
        this.services = services;
        vm = new ProductPageViewModel(services);
        DataContext = vm;
    }

}
