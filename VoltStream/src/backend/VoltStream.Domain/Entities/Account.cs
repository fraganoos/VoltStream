namespace VoltStream.Domain.Entities;

public class Account : Auditable
{
    public long CustomerId { get; set; }
    public decimal BeginningSumm { get; set; }
    public decimal CurrentSumm { get; set; }
    public decimal DiscountSumm { get; set; }

    public Customer Customer { get; set; } = default!;
    public ICollection<CustomerOperation> CustomerOperations { get; set; } = default!;
    public ICollection<DiscountOperation> DiscountOperations { get; set; } = default!;
}
