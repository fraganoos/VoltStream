namespace VoltStream.Application.Features.DiscountOperations.DTOs;

public record DiscountOperationCommandDto(
    long Id,
    string Description,
    bool IsApplied,
    decimal Amount,
    long CustomerId,
    long SaleId);