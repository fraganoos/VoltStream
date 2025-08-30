namespace VoltStream.Application.Features.DebtKredits.DTOs;

public class DebtKreditDTO
{
    public long Id { get; set; }
    public long CustomerId { get; set; }
    public decimal BeginSumm { get; set; }
    public decimal CurrencySumm { get; set; }
    public bool IsActive { get; set; }
}
