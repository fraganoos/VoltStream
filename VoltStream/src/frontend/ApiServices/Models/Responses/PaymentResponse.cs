namespace ApiServices.Models.Responses;

using ApiServices.Enums;

public class PaymentResponse
{
    public long Id { get; set; }
    public DateTime PaidAt { get; set; } // to'langan sana
    public PaymentType Type { get; set; }
    public decimal Amount { get; set; } // orginal kirim summa
    public decimal ExchangeRate { get; set; } //dollar kurs yoki pul o'tkazish foizi
    public decimal NetAmount { get; set; } // qarz summadan ochiraldigan summa
    public string Description { get; set; } = string.Empty;
    public CurrencyResponse Currency { get; set; } = default!;
    public CustomerResponse Customer { get; set; } = default!;
    public CashOperationResponse CashOperation { get; set; } = default!;
    public CustomerOperationResponse CustomerOperation { get; set; } = default!;
    public DiscountOperationResponse DiscountOperation { get; set; } = default!;
}
