namespace VoltStream.Domain.Entities;

public class Sale : Auditable
{
    public DateTime OperationDate { get; set; } // operation kuni
    public decimal CountRoll { get; set; }    // jami rulonlar soni
    public decimal TotalQuantity { get; set; } // jami metr
    public decimal Summa { get; set; } // jami summa
    public decimal Discount { get; set; } // chegirma
    public string Description { get; set; } = string.Empty;

    public long CustomerId { get; set; }
    public Customer Customer { get; set; } = default!;

    public long CustomerOperationId { get; set; }
    public CustomerOperation CustomerOperation { get; set; } = default!;

    public long DiscountOperationId { get; set; }
    public DiscountOperation DiscountOperation { get; set; } = default!;

    public ICollection<SaleItem> SaleItems { get; set; } = default!;
}