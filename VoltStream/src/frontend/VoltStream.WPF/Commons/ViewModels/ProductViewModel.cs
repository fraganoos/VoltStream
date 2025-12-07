namespace VoltStream.WPF.Commons.ViewModels;

using ApiServices.Models.Responses;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using VoltStream.WPF.Commons;
using VoltStream.WPF.Sales.ViewModels;

public partial class ProductViewModel : ViewModelBase
{
    public long Id { get; set; }
    public long CategoryId { get; set; }

    [ObservableProperty] private string name = string.Empty;
    [ObservableProperty] private CategoryViewModel category = new();
    [ObservableProperty] private ObservableCollection<WarehouseStockResponse> stocks = [];
}