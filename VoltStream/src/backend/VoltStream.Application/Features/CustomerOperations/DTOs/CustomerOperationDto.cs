namespace VoltStream.Application.Features.CustomerOperations.DTOs;

using VoltStream.Domain.Entities;
using VoltStream.Domain.Enums;

public record CustomerOperationDto
{
    public long Id { get; set; }
    public decimal Amount { get; set; }
    public OperationType OperationType { get; set; }
    public string Description { get; set; } = string.Empty;

    public long CurrencyId { get; set; }
    public Currency Currency { get; set; } = default!;

    public long CustomerId { get; set; }
    public Customer Customer { get; set; } = default!;
}
