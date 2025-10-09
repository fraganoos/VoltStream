namespace VoltStream.Application.Features.Cashes.DTOs;

using VoltStream.Domain.Entities;

public class CashOperationDto
{
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public Currency Currency { get; set; } = default!;
    public long CashId { get; set; }
}