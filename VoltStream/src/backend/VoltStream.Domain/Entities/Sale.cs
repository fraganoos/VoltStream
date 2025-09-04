namespace VoltStream.Domain.Entities;

public class Sale : Auditable
{
    public DateTimeOffset OperationDate { get; set; } // operation kuni
    public long CustomerId { get; set; }
    public decimal CountRoll { get; set; }    // jami rulonlar soni
    public decimal TotalQuantity { get; set; } // jami metr
    public decimal Summa { get; set; } // jami summa
    public long CustomerOperationId { get; set; }
    public decimal Discount { get; set; } // chegirma
    public string Description { get; set; } = string.Empty;

    public CustomerOperation CustomerOperation { get; set; } = default!;
    public Customer Customer { get; set; } = default!;

    public ICollection<SaleItem> SaleItems { get; set; } = default!;
}