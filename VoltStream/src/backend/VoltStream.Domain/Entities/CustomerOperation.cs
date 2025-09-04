namespace VoltStream.Domain.Entities;

using VoltStream.Domain.Enums;

public class CustomerOperation : Auditable
{
    public long AccountId { get; set; }
    public decimal Summa { get; set; }
    public OperationType OperationType { get; set; }
    public string Description { get; set; } = string.Empty;

    public Account Account { get; set; } = default!;
}