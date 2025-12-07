namespace VoltStream.Domain.Entities;

using System;
using System.Text.Json.Serialization;
using VoltStream.Domain.Enums;

public class CustomerOperation : Auditable
{
    public decimal Amount { get; set; }
    public OperationType OperationType { get; set; }
    public string Description { get; set; } = string.Empty;
    public long? CustomerId { get; set; }
    public long AccountId { get; set; }
    public Account Account { get; set; } = default!;
    public DateTimeOffset Date { get; set; }

    [JsonIgnore]
    public Sale? Sale { get; set; }
    [JsonIgnore]
    public Payment? Payment { get; set; }
}