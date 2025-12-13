namespace VoltStream.Application.Features.WarehouseStocks.DTOs;

public record WarehouseStockForProductDto
{
    public long Id { get; set; }
    public decimal RollCount { get; set; }
    public decimal LengthPerRoll { get; set; }
    public decimal TotalLength { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountRate { get; set; }

    public long WarehouseId { get; set; }
    public long ProductId { get; set; }
};