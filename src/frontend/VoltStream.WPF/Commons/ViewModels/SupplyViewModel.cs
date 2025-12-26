namespace VoltStream.WPF.Commons.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using VoltStream.WPF.Commons;

public partial class SupplyViewModel : ViewModelBase
{
    public long Id { get; set; }
    public long CategoryId { get; set; }
    public long ProductId { get; set; }
    [ObservableProperty] private DateTimeOffset date = DateTimeOffset.Now;

    [ObservableProperty] private string categoryName = string.Empty;
    [ObservableProperty] private string productName = string.Empty;

    [ObservableProperty] private decimal rollCount;
    [ObservableProperty] private decimal lengthPerRoll;
    [ObservableProperty] private decimal totalLength;

    [ObservableProperty] private string unit = "metr";

    [ObservableProperty] private decimal unitPrice;
    [ObservableProperty] private decimal discountRate;

    [ObservableProperty] private ProductViewModel? product;

    public string DisplayDate => Date.ToString("dd.MM.yyyy");

    partial void OnRollCountChanged(decimal value) => CalculateTotal();
    partial void OnLengthPerRollChanged(decimal value) => CalculateTotal();

    private void CalculateTotal()
    {
        TotalLength = RollCount * LengthPerRoll;
    }
}
