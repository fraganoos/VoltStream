namespace VoltStream.Application.Features.Cashes.DTOs;

using VoltStream.Domain.Entities;

public record CashDto
{
    public long Id { get; set; }
    public decimal Balance { get; set; }
    public bool IsActive { get; set; } = true;

    public Currency Currency { get; set; } = default!;

    public ICollection<CashOperationDto> CashOperations { get; set; } = default!;
}
