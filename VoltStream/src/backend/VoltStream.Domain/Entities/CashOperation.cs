namespace VoltStream.Domain.Entities;

using VoltStream.Domain.Enums;

public class CashOperation : Auditable
{
    public DateTime Date { get; set; }
    public decimal Summa { get; set; }
    public CurrencyType CurrencyType { get; set; }
    public string Description { get; set; } = string.Empty;
    public long CashId { get; set; }

    public Cash Cash { get; set; } = default!;
}
