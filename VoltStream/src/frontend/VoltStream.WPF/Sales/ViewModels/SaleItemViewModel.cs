namespace VoltStream.WPF.Sales.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using VoltStream.WPF.Commons;

public partial class SaleItemViewModel : ViewModelBase
{
    [ObservableProperty] private long id;
    [ObservableProperty] private long saleId;
    [ObservableProperty] private int? rollCount;
    [ObservableProperty] private decimal? lengthPerRoll;
    [ObservableProperty] private decimal? totalLength;
    [ObservableProperty] private decimal? unitPrice;
    [ObservableProperty] private decimal? discountRate;
    [ObservableProperty] private decimal? discountAmount;
    [ObservableProperty] private decimal? totalAmount;
    [ObservableProperty] private decimal? finalAmount;
}
