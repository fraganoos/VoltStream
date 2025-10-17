namespace ApiServices.Models.Requests;

public record CashRequest
{
    public long Id { get; set; }
    public decimal Balance { get; set; }
    public bool IsActive { get; set; }
    public long CurrencyId { get; set; }
}