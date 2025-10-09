namespace VoltStream.Application.Features.Payments.DTOs;

using VoltStream.Application.Features.Cashes.DTOs;
using VoltStream.Application.Features.Currencies.DTOs;
using VoltStream.Application.Features.CustomerOperations.DTOs;
using VoltStream.Application.Features.Customers.DTOs;
using VoltStream.Application.Features.DiscountOperations.DTOs;
using VoltStream.Domain.Enums;

public record PaymentDto
{
    public DateTime PaidAt { get; set; } // to'langan sana
    public PaymentType Type { get; set; }
    public decimal Amount { get; set; } // orginal kirim summa
    public decimal ExchangeRate { get; set; } //dollar kurs yoki pul o'tkazish foizi
    public decimal NetAmount { get; set; } // qarz summadan ochiraldigan summa
    public string Description { get; set; } = string.Empty;
    public CurrencyDto Currency { get; set; } = default!;
    public CustomerDto Customer { get; set; } = default!;
    public CashOperationDto CashOperation { get; set; } = default!;
    public CustomerOperationDto CustomerOperation { get; set; } = default!;
    public DiscountOperationDto DiscountOperation { get; set; } = default!;
}
