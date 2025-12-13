namespace VoltStream.Domain.Entities;

using VoltStream.Domain.Enums;

public class Payment : Auditable
{
    public DateTimeOffset PaidAt { get; set; }
    public PaymentType Type { get; set; }
    public decimal Amount { get; set; }
    public decimal ExchangeRate { get; set; }
    public decimal NetAmount { get; set; }
    public string Description { get; set; } = string.Empty;

    public long CurrencyId { get; set; }
    public Currency Currency { get; set; } = default!;

    public long CustomerId { get; set; }
    public Customer Customer { get; set; } = default!;

    public long CustomerOperationId { get; set; }
    public CustomerOperation CustomerOperation { get; set; } = default!;
}