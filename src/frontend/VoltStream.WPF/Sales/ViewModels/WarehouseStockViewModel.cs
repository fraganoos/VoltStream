namespace VoltStream.WPF.Sales.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using VoltStream.WPF.Commons;
using VoltStream.WPF.Commons.ViewModels;

public partial class WarehouseStockViewModel : ViewModelBase
{
    public long Id { get; set; }
    public long WarehouseId { get; set; }

    [ObservableProperty] private decimal rollCount;
    [ObservableProperty] private decimal lengthPerRoll;
    [ObservableProperty] private decimal totalLength;
    [ObservableProperty] private decimal unitPrice;
    [ObservableProperty] private decimal discountRate;
    [ObservableProperty] private ProductViewModel product = new();
}