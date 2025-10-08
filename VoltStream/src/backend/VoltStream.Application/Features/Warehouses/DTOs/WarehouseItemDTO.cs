namespace VoltStream.Application.Features.Warehouses.DTOs;

using VoltStream.Application.Features.Products.DTOs;

public record WarehouseItemDto
{
    public long Id { get; set; }
    public long ProductId { get; set; }
    public decimal CountRoll { get; set; }  // rulon soni
    public decimal QuantityPerRoll { get; set; }  //rulon uzunligi
    public decimal Price { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal TotalQuantity { get; set; } // jami uzunlik
    public ProductDto Product { get; set; } = default!;

}
