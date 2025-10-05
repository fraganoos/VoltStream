namespace VoltStream.WPF.Sales.Models;

using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using VoltStream.WPF.Commons;

public partial class Sale : ViewModelBase
{
    [ObservableProperty] private decimal? totalSum;
    [ObservableProperty] private decimal? totalDiscount;
    [ObservableProperty] private decimal? finalSum;

    [ObservableProperty] private DateTime operationDate = DateTime.Now;
    [ObservableProperty] private long customerId;
    [ObservableProperty] private string customerName = string.Empty;
    [ObservableProperty] private string currencyType = string.Empty;
    [ObservableProperty] private bool checkedDiscount = false;
    [ObservableProperty] private string description = string.Empty;
    [ObservableProperty] private string categoryName = string.Empty;
    [ObservableProperty] private long categoryId;
    [ObservableProperty] private long productId;
    [ObservableProperty] private string? productName;
    [ObservableProperty] private string? perRollCount;
    [ObservableProperty] private string? rollCount;
    [ObservableProperty] private decimal warehouseCountRoll = decimal.Zero;
    [ObservableProperty] private string? quantity;
    [ObservableProperty] private decimal newQuantity = decimal.Zero;
    [ObservableProperty] private decimal warehouseQuantity = decimal.Zero;
    [ObservableProperty] private string? price;
    [ObservableProperty] private string? sum;
    [ObservableProperty] private string? perDiscount;
    [ObservableProperty] private string? discount;
    [ObservableProperty] private string? finalSumProduct;

    public ObservableCollection<SaleItem> SaleItems { get; set; } = [];
}
