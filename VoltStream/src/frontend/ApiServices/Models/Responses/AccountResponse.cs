namespace ApiServices.Models.Responses;

public class AccountResponse
{
    public long Id { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal Balance { get; set; }
    public decimal Discount { get; set; }
    public long CustomerId { get; set; }

    public CurrencyResponse Currency { get; set; } = default!;
}
