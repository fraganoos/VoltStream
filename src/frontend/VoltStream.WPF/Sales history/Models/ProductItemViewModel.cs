namespace VoltStream.WPF.Sales_history.Models;

using CommunityToolkit.Mvvm.ComponentModel;
using VoltStream.WPF.Commons;

public partial class ProductItemViewModel : ViewModelBase
{
    [ObservableProperty] private DateTimeOffset? operationDate;
    [ObservableProperty] private string? category = string.Empty;
    [ObservableProperty] private string? name = string.Empty;
    [ObservableProperty] private decimal? rollLength;
    [ObservableProperty] private decimal? quantity;
    [ObservableProperty] private int? totalCount;
    [ObservableProperty] private string? unit = "metr";
    [ObservableProperty] private decimal? price;
    [ObservableProperty] private decimal? totalAmount;
    [ObservableProperty] private string? customer = string.Empty;

    partial void OnPriceChanged(decimal? value)
        => ReCalculateTotalAmount();

    partial void OnTotalCountChanged(int? value)
        => ReCalculateTotalAmount();

    private void ReCalculateTotalAmount()
    {
        if (Price > 0)
            TotalAmount = TotalCount * Price;
    }
}