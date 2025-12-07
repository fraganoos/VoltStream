namespace VoltStream.Application.Features.Payments.DTOs;

using VoltStream.Application.Features.Cashes.DTOs;
using VoltStream.Application.Features.Currencies.DTOs;
using VoltStream.Application.Features.CustomerOperations.DTOs;
using VoltStream.Application.Features.Customers.DTOs;
using VoltStream.Application.Features.DiscountOperations.DTOs;
using VoltStream.Domain.Enums;

public record PaymentDto
{
    public long Id { get; set; }
    public DateTimeOffset PaidAt { get; set; } // to'langan sana
    public PaymentType Type { get; set; }
    public decimal Amount { get; set; } // orginal kirim summa
    public decimal ExchangeRate { get; set; } //dollar kurs yoki pul o'tkazish foizi
    public decimal NetAmount { get; set; } // qarz summadan ochiraldigan summa
    public string Description { get; set; } = string.Empty;

    public long CurrencyId { get; set; }
    public CurrencyDto Currency { get; set; } = default!;

    public long CustomerId { get; set; }
    public CustomerDto Customer { get; set; } = default!;

    public long CashOperationId { get; set; }
    public CashOperationDto CashOperation { get; set; } = default!;

    public long CustomerOperationId { get; set; }
    public CustomerOperationForSaleDto CustomerOperation { get; set; } = default!;

    public long DiscountOperationId { get; set; }
    public DiscountOperationForSaleDto DiscountOperation { get; set; } = default!;
}

public record PaymentForDiscountOperationDto
{
    public long Id { get; set; }
    public DateTimeOffset PaidAt { get; set; } // to'langan sana
    public PaymentType Type { get; set; }
    public decimal Amount { get; set; } // orginal kirim summa
    public decimal ExchangeRate { get; set; } //dollar kurs yoki pul o'tkazish foizi
    public decimal NetAmount { get; set; } // qarz summadan ochiraldigan summa
    public string Description { get; set; } = string.Empty;

    public long CurrencyId { get; set; }
    public CurrencyDto Currency { get; set; } = default!;

    public long CustomerId { get; set; }
    public CustomerForOperationDto Customer { get; set; } = default!;

    public long CashOperationId { get; set; }
    public CashOperationDto CashOperation { get; set; } = default!;

    public long CustomerOperationId { get; set; }
    public CustomerOperationForSaleDto CustomerOperation { get; set; } = default!;

    public long DiscountOperationId { get; set; }
}

public record PaymentForCustomerOperationDto
{
    public long Id { get; set; }
    public DateTimeOffset PaidAt { get; set; } // to'langan sana
    public PaymentType Type { get; set; }
    public decimal Amount { get; set; } // orginal kirim summa
    public decimal ExchangeRate { get; set; } //dollar kurs yoki pul o'tkazish foizi
    public decimal NetAmount { get; set; } // qarz summadan ochiraldigan summa
    public string Description { get; set; } = string.Empty;

    public long CurrencyId { get; set; }
    public CurrencyDto Currency { get; set; } = default!;

    public long CustomerId { get; set; }
    public CustomerForOperationDto Customer { get; set; } = default!;

    public long CashOperationId { get; set; }
    public CashOperationDto CashOperation { get; set; } = default!;

    public long CustomerOperationId { get; set; }

    public long DiscountOperationId { get; set; }
    public DiscountOperationForSaleDto DiscountOperation { get; set; } = default!;
}