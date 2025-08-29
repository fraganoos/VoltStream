namespace VoltStream.Application.Features.Payments.DTOs;

using VoltStream.Domain.Enums;

public record PaymentDTO
{
    public long Id { get; set; }
    public DateTimeOffset PaidDate { get; set; }
    public long CustomerId { get; set; }
    public PaymentType PaymentType { get; set; }
    public decimal Summa { get; set; }
    public CurrencyType CurrencyType { get; set; } = default!;
    public decimal Kurs { get; set; }
    public decimal DefaultSumm { get; set; }
    public string Description { get; set; } = null!;
    public long CustomerOperation { get; set; }
}