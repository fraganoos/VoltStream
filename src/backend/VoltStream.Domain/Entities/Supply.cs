namespace VoltStream.Domain.Entities;

public class Supply : Auditable
{
    public DateTimeOffset Date { get; set; }
    public int RollCount { get; set; }
    public decimal LengthPerRoll { get; set; }
    public decimal TotalLength { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountRate { get; set; }

    public long ProductId { get; set; }
    public Product Product { get; set; } = default!;
}
