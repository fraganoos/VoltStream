namespace VoltStream.Application.Features.Warehouses.DTOs;

using VoltStream.Application.Features.WarehouseStocks.DTOs;

public record WarehouseDto(
    long Id,
    string Name,
    ICollection<WarehouseStockDto> Stocks
);