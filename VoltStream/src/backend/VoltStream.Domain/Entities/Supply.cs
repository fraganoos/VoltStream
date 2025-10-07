namespace VoltStream.Domain.Entities;

public class Supply : Auditable
{
    public DateTime OperationDate { get; set; }
    public long ProductId { get; set; }
    public decimal CountRoll { get; set; }
    public decimal QuantityPerRoll { get; set; }
    public decimal TotalQuantity { get; set; }

    public Product Product { get; set; } = default!;
}
