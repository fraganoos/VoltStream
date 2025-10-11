namespace VoltStream.Application.Features.WarehouseStocks.DTOs;

using VoltStream.Application.Features.Categories.DTOs;

public record ProductForWarehouseDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public long CategoryId { get; set; }
    public CategoryDto? Category { get; set; } = default!;
}
