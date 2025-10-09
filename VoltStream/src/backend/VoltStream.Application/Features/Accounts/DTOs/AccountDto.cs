namespace VoltStream.Application.Features.Accounts.DTOs;

using VoltStream.Application.Features.Currencies.DTOs;

public record AccountDto(
    long Id,
    decimal OpeningBalance,
    decimal Balance,
    decimal Discount,
    long CustomerId,
    CurrencyDto Currency);
