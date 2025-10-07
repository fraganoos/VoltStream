namespace VoltStream.Domain.Entities;

public class Sale : Auditable
{
    public DateTime Date { get; set; } // operation kuni
    public decimal RollCount { get; set; }    // jami rulonlar soni
    public decimal Length { get; set; } // jami metr
    public decimal Amount { get; set; } // jami summa
    public decimal Discount { get; set; } // chegirma
    public string Description { get; set; } = string.Empty;

    public long CurrencyId { get; set; }
    public Currency Currency { get; set; } = default!;

    public long CustomerId { get; set; }
    public Customer Customer { get; set; } = default!;

    public long CustomerOperationId { get; set; }
    public CustomerOperation CustomerOperation { get; set; } = default!;

    public long DiscountOperationId { get; set; }
    public DiscountOperation DiscountOperation { get; set; } = default!;

    public ICollection<SaleItem> SaleItems { get; set; } = default!;
}