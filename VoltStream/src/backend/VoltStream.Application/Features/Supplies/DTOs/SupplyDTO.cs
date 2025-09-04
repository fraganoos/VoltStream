namespace VoltStream.Application.Features.Supplies.DTOs;

using VoltStream.Application.Features.Products.DTOs;

public record SupplyDTO
{
    public long Id { get; set; }
    public DateTimeOffset OperationDate { get; set; }
    public long ProductId { get; set; }
    public decimal CountRoll { get; set; }
    public decimal QuantityPerRoll { get; set; }
    public decimal TotalQuantity { get; set; }
    public ProductDTO Product { get; set; } = default!;
}
