namespace VoltStream.WPF.Sales.Models;

using CommunityToolkit.Mvvm.ComponentModel;
using VoltStream.WPF.Commons;

public partial class SaleItem : ViewModelBase
{
    [ObservableProperty] private long categoryId;
    [ObservableProperty] private string categoryName = string.Empty;
    [ObservableProperty] private long productId;
    [ObservableProperty] private string productName = string.Empty;
    [ObservableProperty] private decimal? perRollCount;
    [ObservableProperty] private decimal warehouseQuantity = decimal.Zero;
    [ObservableProperty] private decimal? rollCount;
    [ObservableProperty] private decimal warehouseCountRoll = decimal.Zero;
    [ObservableProperty] private decimal? quantity;
    [ObservableProperty] private decimal newQuantity = decimal.Zero;
    [ObservableProperty] private decimal? price;
    [ObservableProperty] private decimal? sum;
    [ObservableProperty] private decimal? perDiscount;
    [ObservableProperty] private decimal? discount;
    [ObservableProperty] private decimal? finalSumProduct;
}
