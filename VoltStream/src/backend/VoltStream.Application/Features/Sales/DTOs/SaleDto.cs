namespace VoltStream.Application.Features.Sales.DTOs;

using VoltStream.Application.Features.Currencies.DTOs;
using VoltStream.Application.Features.CustomerOperations.DTOs;
using VoltStream.Application.Features.Customers.DTOs;
using VoltStream.Application.Features.DiscountOperations.DTOs;

public record SaleDto(
    long Id,
    DateTime Date, // operation kuni
    decimal RollCount, // jami rulonlar soni
    decimal Length, // butun savdo bo'yicha jami uzunlik
    decimal Amount, // jami narxi
    decimal Discount, // chegirma narxi
    string Description,
    CustomerOperationDto CustomerOperation,
    DiscountOperationDto DiscountOperation,
    CurrencyDto Currency,
    CustomerDto Customer,
    ICollection<SaleItemDto> Items
);
