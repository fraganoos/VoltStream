namespace ApiServices.Models.Requests;

public sealed record ApplyDiscountRequest
{
    public DateTime Date { get; set; }
    public long CustomerId { get; set; }
    public decimal DiscountAmount { get; set; }
    public bool IsCash { get; set; }
    public string Description { get; set; } = string.Empty;
}