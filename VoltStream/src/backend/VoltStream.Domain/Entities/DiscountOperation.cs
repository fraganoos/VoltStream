namespace VoltStream.Domain.Entities;

public class DiscountOperation : Auditable
{
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsApplied { get; set; }
    public decimal Amount { get; set; }

    public long CustomerId { get; set; }
    public Customer Customer { get; set; } = default!;
}