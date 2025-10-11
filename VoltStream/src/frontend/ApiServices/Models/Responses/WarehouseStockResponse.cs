namespace ApiServices.Models.Responses;

public class WarehouseStockResponse
{
    public long Id { get; set; }
    public decimal RollCount { get; set; }  // rulon soni
    public decimal LengthPerRoll { get; set; }  //rulon uzunligi
    public decimal TotalLength { get; set; } // jami uzunlik
    public decimal UnitPrice { get; set; }
    public decimal DiscountRate { get; set; }
    public long WarehouseId { get; set; }

    public long ProductId { get; set; }
    public ProductResponse Product { get; set; } = default!;
}