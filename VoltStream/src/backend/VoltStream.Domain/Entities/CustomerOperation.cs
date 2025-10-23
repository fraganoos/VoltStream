namespace VoltStream.Domain.Entities;

using VoltStream.Domain.Enums;

public class CustomerOperation : Auditable
{
    public decimal Amount { get; set; }
    public OperationType OperationType { get; set; }
    public string Description { get; set; } = string.Empty;
    public long? CustomerId { get; set; }
    public long AccountId { get; set; }
    public Account Account { get; set; } = default!;
}