namespace VoltStream.Application.Features.Sales.DTOs;

using VoltStream.Application.Features.Products.DTOs;

public record SaleItemDto(
    long Id,
    decimal RollCount,  // shu turga tegishli rulonlar soni
    decimal LengthPerRoll,  // bir rulondagi uzunlik
    decimal TotalLength, // shu turga tegishli jami uzunlik
    decimal UnitPrice, // shu tur bo'yicha 1 metr kabelni narxi
    decimal DiscountRate, // chegirma bahosi foizda %
    decimal DiscountAmount, // chegirma qiymati narxi
    decimal TotalAmount, // shu tur bo'yicha olinayotgan kabelni jami narxi
    long SaleId,
    ProductDto Product
);
