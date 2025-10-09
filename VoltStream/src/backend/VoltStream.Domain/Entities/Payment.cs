namespace VoltStream.Domain.Entities;

using VoltStream.Domain.Enums;

public class Payment : Auditable
{
    public DateTime PaidAt { get; set; } // to'langan sana
    public PaymentType Type { get; set; }
    public decimal Amount { get; set; } // orginal kirim summa
    public decimal ExchangeRate { get; set; } //dollar kurs yoki pul o'tkazish foizi
    public decimal NetAmount { get; set; } // qarz summadan ochiraldigan summa
    public string Description { get; set; } = string.Empty;

    public long CurrencyId { get; set; }
    public Currency Currency { get; set; } = default!;

    public long CustomerId { get; set; }
    public Customer Customer { get; set; } = default!;

    public long CashOperationId { get; set; }
    public CashOperation CashOperation { get; set; } = default!;

    public long CustomerOperationId { get; set; }
    public CustomerOperation CustomerOperation { get; set; } = default!;

    public long DiscountOperationId { get; set; }
    public DiscountOperation DiscountOperation { get; set; } = default!;
}