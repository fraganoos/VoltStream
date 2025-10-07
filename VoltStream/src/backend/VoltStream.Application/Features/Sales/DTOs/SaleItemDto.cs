namespace VoltStream.Application.Features.Sales.DTOs;

using VoltStream.Application.Features.Products.DTOs;

public class SaleItemDto
{
    public long Id { get; set; }
    public long SaleId { get; set; }
    public decimal RollCount { get; set; }  // entity: SaleItem.RollCount
    public decimal LengthPerRoll { get; set; }  // entity: SaleItem.LengthPerRoll
    public decimal TotalLength { get; set; } // entity: SaleItem.TotalLength
    public decimal UnitPrice { get; set; } // entity: SaleItem.UnitPrice
    public decimal DiscountRate { get; set; } // entity: SaleItem.DiscountRate
    public decimal DiscountAmount { get; set; } // entity: SaleItem.DiscountAmount
    public decimal TotalAmount { get; set; } // entity: SaleItem.TotalAmount

    public long ProductId { get; set; }
    public ProductDTO Product { get; set; } = default!;
}

