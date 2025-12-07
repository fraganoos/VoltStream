namespace VoltStream.WPF.Sales.ViewModels;

using ApiServices.Models.Responses;
using CommunityToolkit.Mvvm.ComponentModel;
using VoltStream.WPF.Commons;

public partial class WarehouseStockViewModel : ViewModelBase
{
    public long Id { get; set; }
    [ObservableProperty] private decimal rollCount;  // rulon soni
    [ObservableProperty] private decimal lengthPerRoll;  //rulon uzunligi
    [ObservableProperty] private decimal totalLength; // jami uzunlik
    [ObservableProperty] private decimal unitPrice;
    [ObservableProperty] private decimal discountRate;
    [ObservableProperty] private long warehouseId;
    [ObservableProperty] private ProductResponse product = new();
}