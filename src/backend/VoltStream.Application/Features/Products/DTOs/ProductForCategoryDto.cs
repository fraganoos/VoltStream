namespace VoltStream.Application.Features.Products.DTOs;

using VoltStream.Application.Features.WarehouseStocks.DTOs;

public record ProductForCategoryDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;

    public long CategoryId { get; set; }
    public ICollection<WarehouseStockForProductDto>? Stocks { get; set; } = default!;
}