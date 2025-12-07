namespace ApiServices.Models.Responses;

using ApiServices.Enums;

public class PaymentResponse
{
    public long Id { get; set; }
    public DateTimeOffset PaidAt { get; set; } // to'langan sana
    public PaymentType Type { get; set; }
    public decimal Amount { get; set; } // orginal kirim summa
    public decimal ExchangeRate { get; set; } //dollar kurs yoki pul o'tkazish foizi
    public decimal NetAmount { get; set; } // qarz summadan ochiraldigan summa
    public string Description { get; set; } = string.Empty;

    public long CurrencyId { get; set; }
    public CurrencyResponse Currency { get; set; } = default!;

    public long CustomerId { get; set; }
    public CustomerResponse Customer { get; set; } = default!;

    public long CashOperationId { get; set; }
    public CashOperationResponse CashOperation { get; set; } = default!;

    public long CustomerOperationId { get; set; }
    public CustomerOperationResponse CustomerOperation { get; set; } = default!;

    public long DiscountOperationId { get; set; }
    public DiscountOperationResponse DiscountOperation { get; set; } = default!;
}
