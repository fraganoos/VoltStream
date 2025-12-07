namespace VoltStream.Application.Features.Sales.DTOs;

using VoltStream.Application.Features.Currencies.DTOs;
using VoltStream.Application.Features.CustomerOperations.DTOs;
using VoltStream.Application.Features.Customers.DTOs;
using VoltStream.Application.Features.DiscountOperations.DTOs;

public record SaleDto
{
    public long Id { get; set; }
    public DateTimeOffset Date { get; set; } // operation kuni
    public int RollCount { get; set; } // jami rulonlar soni
    public decimal Length { get; set; } // butun savdo bo'yicha jami uzunlik
    public decimal Amount { get; set; } // jami narxi
    public decimal Discount { get; set; } // chegirma narxi
    public string Description { get; set; } = string.Empty;

    public long CustomerOperationId { get; set; }
    public CustomerOperationForSaleDto CustomerOperation { get; set; } = default!;

    public long DiscountOperationId { get; set; }
    public DiscountOperationForSaleDto DiscountOperation { get; set; } = default!;

    public long CurrencyId { get; set; }
    public CurrencyDto Currency { get; set; } = default!;

    public long CustomerId { get; set; }
    public CustomerDto Customer { get; set; } = default!;

    public ICollection<SaleItemDto> Items { get; set; } = default!;
}

public record SaleForCustomerOperationDto
{
    public long Id { get; set; }
    public DateTimeOffset Date { get; set; } // operation kuni
    public int RollCount { get; set; } // jami rulonlar soni
    public decimal Length { get; set; } // butun savdo bo'yicha jami uzunlik
    public decimal Amount { get; set; } // jami narxi
    public decimal Discount { get; set; } // chegirma narxi
    public string Description { get; set; } = string.Empty;

    public long DiscountOperationId { get; set; }
    public DiscountOperationForSaleDto DiscountOperation { get; set; } = default!;

    public long CurrencyId { get; set; }
    public CurrencyDto Currency { get; set; } = default!;

    public long CustomerId { get; set; }
    public CustomerForOperationDto Customer { get; set; } = default!;

    public ICollection<SaleItemDto> Items { get; set; } = default!;
}

public record SaleForDiscountOperationDto
{
    public long Id { get; set; }
    public DateTimeOffset Date { get; set; } // operation kuni
    public int RollCount { get; set; } // jami rulonlar soni
    public decimal Length { get; set; } // butun savdo bo'yicha jami uzunlik
    public decimal Amount { get; set; } // jami narxi
    public decimal Discount { get; set; } // chegirma narxi
    public string Description { get; set; } = string.Empty;

    public long CustomerOperationId { get; set; }
    public CustomerOperationForSaleDto CustomerOperation { get; set; } = default!;

    public long CurrencyId { get; set; }
    public CurrencyDto Currency { get; set; } = default!;

    public long CustomerId { get; set; }
    public CustomerForOperationDto Customer { get; set; } = default!;

    public ICollection<SaleItemDto> Items { get; set; } = default!;
}