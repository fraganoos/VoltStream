namespace VoltStream.Domain.Entities;

using VoltStream.Domain.Enums;

public class CustomerOperation : Auditable
{
    public decimal Amount { get; set; }
    public OperationType OperationType { get; set; }
    public string Description { get; set; } = string.Empty;

    public long AccountId { get; set; }
    public Account Account { get; set; } = default!;

    public long SaleId { get; set; }
    public Sale Sale { get; set; } = default!;
}