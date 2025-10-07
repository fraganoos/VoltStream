namespace VoltStream.Application.Features.Payments.DTOs;

using VoltStream.Application.Features.Currencies.DTOs;
using VoltStream.Domain.Enums;

public record PaymentDto
{
    public long Id { get; set; }
    public DateTime PaidAt { get; set; }
    public PaymentType Type { get; set; }
    public decimal Amount { get; set; }
    public decimal ExchangeRate { get; set; }
    public decimal NetAmount { get; set; }
    public string Description { get; set; } = string.Empty;
    public long CustomerOperationId { get; set; }

    public long CurrencyId { get; set; }
    public CurrencyDto Currency { get; set; } = default!;
}
