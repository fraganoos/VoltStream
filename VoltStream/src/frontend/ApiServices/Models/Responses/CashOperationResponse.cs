namespace ApiServices.Models.Responses;

public class CashOperationResponse
{
    public long Id { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public CurrencyResponse Currency { get; set; } = default!;
    public long CashId { get; set; }
}