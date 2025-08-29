namespace VoltStream.Domain.Entities;
public class CashOperation : Auditable
{
    public DateTimeOffset Date { get; set; }
    public decimal Summa { get; set; }
    public Enums.CurrencyType CurrencyType { get; set; }
    public string Description { get; set; } = string.Empty;
}
