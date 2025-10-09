namespace VoltStream.Application.Features.WarehouseStocks.DTOs;

using VoltStream.Application.Features.Products.DTOs;

public record WarehouseStockDto(
    long Id,
    decimal RollCount,  // rulon soni
    decimal LengthPerRoll,  //rulon uzunligi
    decimal TotalLength, // jami uzunlik
    decimal UnitPrice,
    decimal DiscountRate,
    long WarehouseId,
    ProductDto Product
);
