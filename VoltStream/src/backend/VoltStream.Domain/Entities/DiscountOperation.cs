namespace VoltStream.Domain.Entities;

public class DiscountOperation : Auditable
{
    public DateTimeOffset Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsApplied { get; set; }
    public decimal Amount { get; set; }

    public long AccountId { get; set; }
    public Account Account { get; set; } = default!;
}