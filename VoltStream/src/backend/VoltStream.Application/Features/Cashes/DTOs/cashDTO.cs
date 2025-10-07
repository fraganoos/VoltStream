namespace VoltStream.Application.Features.Cashes.DTOs;

using VoltStream.Domain.Entities;

public record CashDTO
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Balance { get; set; }
    public bool IsActive { get; set; } = true;

    public long CurrencyId { get; set; }
    public Currency Currency { get; set; } = default!;

    public ICollection<CashOperation> CashOperations { get; set; } = default!;
}
