namespace VoltStream.Domain.Entities;

public class CashOperation : Auditable
{
    public DateTimeOffset Date { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;

    public long CurrencyId { get; set; }
    public Currency Currency { get; set; } = default!;

    public long CashId { get; set; }
    public Cash Cash { get; set; } = default!;
}
