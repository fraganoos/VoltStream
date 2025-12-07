namespace VoltStream.Domain.Entities;

public class WarehouseStock : Auditable
{
    public decimal RollCount { get; set; }  // rulon soni
    public decimal LengthPerRoll { get; set; }  //rulon uzunligi
    public decimal TotalLength { get; set; } // jami uzunlik
    public decimal UnitPrice { get; set; }
    public decimal DiscountRate { get; set; }

    public long ProductId { get; set; }
    public Product Product { get; set; } = default!;

    public long WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = default!;
}
