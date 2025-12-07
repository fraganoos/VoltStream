namespace VoltStream.Domain.Entities;

using System.Text.Json.Serialization;

public class DiscountOperation : Auditable
{
    public DateTimeOffset Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsApplied { get; set; }
    public decimal Amount { get; set; }
    public long? CustomerId { get; set; }
    public long AccountId { get; set; }
    public Account Account { get; set; } = default!;

    [JsonIgnore]
    public Sale? Sale { get; set; }
    [JsonIgnore]
    public Payment? Payment { get; set; }
}