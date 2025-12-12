namespace VoltStream.Domain.Entities;

using System.Text.Json.Serialization;
using VoltStream.Domain.Enums;

public class CustomerOperation : Auditable
{
    public DateTimeOffset Date { get; set; }
    public decimal Amount { get; set; }
    public decimal Discount { get; set; }
    public bool IsDiscountApplied { get; set; }
    public OperationType OperationType { get; set; }
    public string Description { get; set; } = string.Empty;
    public long? CustomerId { get; set; }

    public long AccountId { get; set; }
    public Account Account { get; set; } = default!;

    [JsonIgnore]
    public Sale? Sale { get; set; }

    [JsonIgnore]
    public Payment? Payment { get; set; }
}