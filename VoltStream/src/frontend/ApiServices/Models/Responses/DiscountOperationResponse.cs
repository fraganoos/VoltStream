namespace ApiServices.Models.Responses;

public class DiscountOperationResponse
{
    public long Id { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsApplied { get; set; }
    public decimal Amount { get; set; }

    public long CustomerId { get; set; }
    public long SaleId { get; set; }
}