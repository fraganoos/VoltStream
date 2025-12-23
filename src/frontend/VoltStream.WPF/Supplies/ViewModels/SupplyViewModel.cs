namespace VoltStream.WPF.Supplies.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using VoltStream.WPF.Commons;
using VoltStream.WPF.Commons.ViewModels;

public partial class SupplyViewModel : ViewModelBase
{
    [ObservableProperty] private long id;
    [ObservableProperty] private DateTimeOffset date = DateTimeOffset.Now;
    
    [ObservableProperty] private long categoryId;
    [ObservableProperty] private string categoryName = string.Empty;
    
    [ObservableProperty] private long productId;
    [ObservableProperty] private string productName = string.Empty;
    
    [ObservableProperty] private decimal rollCount;
    [ObservableProperty] private decimal lengthPerRoll;
    [ObservableProperty] private decimal totalLength;
    
    [ObservableProperty] private string unit = "metr";
    
    [ObservableProperty] private decimal price;
    [ObservableProperty] private decimal discountRate;

    [ObservableProperty] private ProductViewModel product;

    // Helper property for UI display if needed, or strict binding
    public string DisplayDate => Date.ToString("dd.MM.yyyy");

    partial void OnRollCountChanged(decimal value) => CalculateTotal();
    partial void OnLengthPerRollChanged(decimal value) => CalculateTotal();

    private void CalculateTotal()
    {
        TotalLength = RollCount * LengthPerRoll;
    }
}
