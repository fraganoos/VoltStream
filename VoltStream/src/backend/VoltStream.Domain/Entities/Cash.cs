namespace VoltStream.Domain.Entities;

public class Cash : Auditable
{
    public string Name { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public bool IsActive { get; set; } = true;

    public long CurrencyId { get; set; }
    public Currency Currency { get; set; } = default!;

    public ICollection<CashOperation> CashOperations { get; set; } = default!;
}
