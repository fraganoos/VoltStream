namespace VoltStream.Domain.Entities;

using VoltStream.Domain.Enums;

public class Payment : Auditable
{
    public DateTimeOffset PaidDate { get; set; } // to'langan sana
    public PaymentType PaymentType { get; set; }
    public decimal Summa { get; set; } // orginal kirim summa
    public CurrencyType CurrencyType { get; set; } = default!;
    public decimal Kurs { get; set; } //dollar kurs yoki pul o'tkazish foizi
    public decimal DefaultSumm { get; set; } // qarz summadan ochiraldigan summa
    public string Description { get; set; } = string.Empty;

    public long AccountId { get; set; }
    public Account Account { get; set; } = default!;

    public long CashOperationId { get; set; }
    public CashOperation CashOperation { get; set; } = default!;

    public long CustomerOperationId { get; set; }
    public CustomerOperation CustomerOperation { get; set; } = default!;
}