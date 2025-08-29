namespace VoltStream.Domain.Entities;

public class Warehouse : Auditable
{
    public long ProductId { get; set; }
    public decimal CountRoll { get; set; }  // rulon soni
    public decimal QuantityPerRoll { get; set; }  //rulon uzunligi
    public decimal TotalQuantity { get; set; } // jami uzunlik

    public Product Product { get; set; } = default!;
}