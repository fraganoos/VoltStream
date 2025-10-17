namespace ApiServices.Models.Requests;

public record AccountRequest
{
    public long Id { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal Balance { get; set; }
    public decimal Discount { get; set; }
    public long CustomerId { get; set; }
    public long CurrencyId { get; set; }
}