namespace ApiServices.Models.Reqiuests;

using ApiServices.Enums;

public record PaymentRequest
{
    public long Id { get; set; }
    public DateTime PaidAt { get; set; }
    public PaymentType Type { get; set; }
    public decimal Amount { get; set; }
    public decimal ExchangeRate { get; set; }
    public decimal NetAmount { get; set; }
    public string Description { get; set; } = string.Empty;
    public long CurrencyId { get; set; }
    public long CustomerId { get; set; }
}