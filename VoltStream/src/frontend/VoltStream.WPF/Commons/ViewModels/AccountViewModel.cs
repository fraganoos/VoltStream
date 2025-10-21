namespace VoltStream.WPF.Commons.ViewModels;

using ApiServices.Models.Responses;
using VoltStream.WPF.Commons;

public class AccountViewModel : ViewModelBase
{
    public long Id { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal Balance { get; set; }
    public decimal Discount { get; set; }
    public long CustomerId { get; set; }
    public long CurrencyId { get; set; }

    public CurrencyResponse Currency { get; set; } = default!;
}