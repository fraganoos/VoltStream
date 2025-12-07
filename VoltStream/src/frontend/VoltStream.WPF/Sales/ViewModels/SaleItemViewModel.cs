namespace VoltStream.WPF.Sales.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using VoltStream.WPF.Commons;
using VoltStream.WPF.Turnovers.Models;

public partial class SaleItemViewModel : ViewModelBase
{
    public long Id { get; set; }
    public long SaleId { get; set; }
    public long ProductId { get; set; }
    [ObservableProperty] private int? rollCount;
    [ObservableProperty] private decimal? lengthPerRoll;
    [ObservableProperty] private decimal? totalLength;
    [ObservableProperty] private decimal? unitPrice;
    [ObservableProperty] private decimal? discountRate;
    [ObservableProperty] private decimal? discountAmount;
    [ObservableProperty] private decimal? totalAmount;
    [ObservableProperty] private decimal? finalAmount;

    [ObservableProperty] private SaleViewModel sale = new();
    [ObservableProperty] private ProductViewModel product = new();
}
