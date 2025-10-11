namespace VoltStream.Domain.Entities;

public class Currency : Auditable
{
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Symbol { get; set; } = string.Empty;
    public decimal ExchangeRate { get; set; }
    public bool IsDefault { get; set; }
}