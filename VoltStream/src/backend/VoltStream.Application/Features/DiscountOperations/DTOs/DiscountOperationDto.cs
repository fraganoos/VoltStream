namespace VoltStream.Application.Features.DiscountOperations.DTOs;

public record DiscountOperationDto(
    long Id,
    DateTime Date,
    string Description,
    bool IsApplied,
    decimal Amount,
    long CustomerId,
    long SaleId);