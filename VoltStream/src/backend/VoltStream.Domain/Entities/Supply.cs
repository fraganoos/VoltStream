namespace VoltStream.Domain.Entities;

public class Supply : Auditable
{
    public DateTime Date { get; set; }
    public decimal RollCount { get; set; }
    public decimal LengthPerRoll { get; set; }
    public decimal TotalLength { get; set; }

    public long ProductId { get; set; }
    public Product Product { get; set; } = default!;
}
