namespace VoltStream.Application.Features.Supplies.DTOs;

using VoltStream.Application.Features.Products.DTOs;

public record SupplyDto(
    long Id,
    DateTime Date, // operation kuni
    decimal RollCount, // jami rulonlar soni
    decimal LengthPerRoll, // bir rulondagi uzunlik
    decimal TotalLength, // butun supply bo'yicha jami uzunlik
    ProductDto Product
);