namespace VoltStream.WPF.Sales.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using VoltStream.WPF.Commons;

public partial class ProductViewModel : ViewModelBase
{
    public long Id { get; set; }
    [ObservableProperty] private string name = string.Empty;

    public long CategoryId { get; internal set; }
    [ObservableProperty] private CategoryViewModel category = new();

    [ObservableProperty] private ObservableCollection<WarehouseStockViewModel> stocks = [];
}
