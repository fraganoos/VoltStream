namespace VoltStream.Application.Features.Sales.DTOs;

public record SaleItemCommandDto(
    long Id,
    long SaleId,
    long ProductId,
    decimal RollCount,
    decimal LengthPerRoll,
    decimal TotalLength,
    decimal UnitPrice,
    decimal DiscountRate,
    decimal DiscountAmount,
    decimal TotalAmount
);
