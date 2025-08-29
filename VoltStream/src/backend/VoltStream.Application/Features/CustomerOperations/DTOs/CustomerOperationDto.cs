namespace VoltStream.Application.Features.CustomerOperations.DTOs;

using VoltStream.Domain.Entities;
using VoltStream.Domain.Enums;

public record CustomerOperationDto
{
    public long Id { get; set; }
    public long CustomerId { get; set; }
    public decimal Summa { get; set; }
    public OperationType OperationType { get; set; }
    public string Description { get; set; } = string.Empty;

    public Customer Customer { get; set; } = default!;
}