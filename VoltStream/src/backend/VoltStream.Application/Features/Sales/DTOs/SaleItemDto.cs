namespace VoltStream.Application.Features.Sales.DTOs;

using VoltStream.Application.Features.Products.DTOs;

public record SaleItemDto
{
    public long Id { get; set; }
    public decimal RollCount { get; set; }  // shu turga tegishli rulonlar soni
    public decimal LengthPerRoll { get; set; }  // bir rulondagi uzunlik
    public decimal TotalLength { get; set; } // shu turga tegishli jami uzunlik
    public decimal UnitPrice { get; set; } // shu tur bo'yicha 1 metr kabelni narxi
    public decimal DiscountRate { get; set; } // chegirma bahosi foizda %
    public decimal DiscountAmount { get; set; } // chegirma qiymati narxi
    public decimal TotalAmount { get; set; } // shu tur bo'yicha olinayotgan kabelni jami narxi { get; set; }
    public decimal FinalAmount { get; set; }
    public long SaleId { get; set; }
    public ProductDto Product { get; set; } = default!;
}