namespace VoltStream.Application.Features.Categories.DTOs;

using VoltStream.Application.Features.WarehouseStocks.DTOs;

public record ProductForCategoryDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public long CategoryId { get; set; }
    public ICollection<WarehouseStockDto>? Stocks { get; set; } = default!;
}