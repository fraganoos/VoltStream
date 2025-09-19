namespace VoltStream.Domain.Entities;

public class DiscountOperation : Auditable
{
    public DateTimeOffset Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Summa { get; set; }
    public bool IsDiscountUsed { get; set; }
    public decimal DiscountSumm { get; set; }

    public long CustomerId { get; set; }
    public Customer Customer { get; set; } = default!;
}