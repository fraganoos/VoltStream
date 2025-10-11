namespace VoltStream.Domain.Entities;

public class DiscountOperation : Auditable
{
    public DateTimeOffset Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsApplied { get; set; }
    public decimal Amount { get; set; }

    public long CustomerId { get; set; }
    public Customer Customer { get; set; } = default!;

    public long SaleId { get; set; }
    public Sale Sale { get; set; } = default!;
}