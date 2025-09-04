namespace VoltStream.Domain.Entities;

public class WarehouseItem : Auditable
{
    public decimal CountRoll { get; set; }  // rulon soni
    public decimal QuantityPerRoll { get; set; }  //rulon uzunligi
    public decimal TotalQuantity { get; set; } // jami uzunlik
    public decimal Price { get; set; }
    public decimal DiscountPercent { get; set; }

    public long ProductId { get; set; }
    public Product Product { get; set; } = default!;

    public long WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = default!;
}
