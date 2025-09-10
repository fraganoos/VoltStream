namespace ApiServices.DTOs.Supplies;

public record WarehouseItem
{
    public long Id { get; set; }
    public long ProductId { get; set; }
    public decimal CountRoll { get; set; }  // rulon soni
    public decimal QuantityPerRoll { get; set; }  //rulon uzunligi
    public decimal Price { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal TotalQuantity { get; set; } // jami uzunlik
}
