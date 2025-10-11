namespace VoltStream.Application.Features.Products.DTOs;

public record ProductDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public long CategoryId { get; set; }
    public CategoryForProductDto? Category { get; set; } = default!;
    public ICollection<WarehouseStockForProductDto>? Stocks { get; set; } = default!;
}
