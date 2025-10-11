namespace VoltStream.Application.Features.CustomerOperations.DTOs;

using VoltStream.Application.Features.Accounts.DTOs;
using VoltStream.Domain.Enums;

public record CustomerOperationDto
{
    public long Id { get; set; }
    public decimal Amount { get; set; }
    public OperationType OperationType { get; set; }
    public string Description { get; set; } = string.Empty;

    public long AccountId { get; set; }
    public AccountDto Account { get; set; } = default!;
}
