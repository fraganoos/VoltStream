namespace VoltStream.Application.Features.Supplies.DTOs;

using VoltStream.Application.Features.Categories.DTOs;
using VoltStream.Application.Features.Products.DTOs;

public record SupplyDto
{
    public long Id { get; set; }
    public DateTimeOffset OperationDate { get; set; }
    public long CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public long ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal CountRoll { get; set; }
    public decimal QuantityPerRoll { get; set; }
    public decimal TotalQuantity { get; set; }
    public decimal DiscountPercent { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public ProductDTO Product { get; set; } = default!;
    public CategoryDto Category { get; set; } = default!;

}
