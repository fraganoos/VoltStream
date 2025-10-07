namespace VoltStream.Application.Features.Supplies.DTOs;

using VoltStream.Application.Features.Products.DTOs;

public record SupplyDto
{
    public long Id { get; set; }
    public DateTime Date { get; set; }
    public decimal RollCount { get; set; }
    public decimal LengthPerRoll { get; set; }
    public decimal TotalLength { get; set; }

    public ProductDTO Product { get; set; } = default!;
}