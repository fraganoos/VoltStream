namespace VoltStream.Application.Features.Accounts.DTOs;

public record AccountCommandDto(
    long Id,
    decimal OpeningBalance,
    decimal Balance,
    decimal Discount,
    long CustomerId,
    long CurrencyId);