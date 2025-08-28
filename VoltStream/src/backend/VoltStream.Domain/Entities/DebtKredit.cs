namespace VoltStream.Domain.Entities;

public class DebtKredit : Auditable
{
    public long CustomerId { get; set; }
    public decimal BeginSumm { get; set; }
    public decimal CurrencySumm { get; set; }
    public bool IsActive { get; set; }
}