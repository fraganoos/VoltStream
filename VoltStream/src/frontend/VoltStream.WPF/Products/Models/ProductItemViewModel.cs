namespace VoltStream.WPF.Products.Models;

using CommunityToolkit.Mvvm.ComponentModel;
using VoltStream.WPF.Commons;

public partial class ProductItemViewModel : ViewModelBase
{
    [ObservableProperty] private string? category = string.Empty;           // Mahsulot turi
    [ObservableProperty] private string? name = string.Empty;               // Nomi
    [ObservableProperty] private decimal? rollLength;                       // Rulon uzunligi
    [ObservableProperty] private int? quantity;                             // Rulon soni
    [ObservableProperty] private int? totalCount;                           // Jami (rulon uzunligi * soni)
    [ObservableProperty] private string? unit = "metr";                     // O‘lchov birligi
    [ObservableProperty] private decimal? price;                            // Narxi
    [ObservableProperty] private decimal? totalAmount;                      // Umumiy summa (narx * jami)

}
