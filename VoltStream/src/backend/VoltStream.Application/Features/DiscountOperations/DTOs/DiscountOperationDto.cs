namespace VoltStream.Application.Features.DiscountOperations.DTOs;

using VoltStream.Domain.Entities;

public record DiscountOperationDto
{
    public long Id { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Summa { get; set; }
    public bool IsDiscountUsed { get; set; }
    public decimal DiscountSumm { get; set; }

    public long AccountId { get; set; }
    public Account Account { get; set; } = default!;
}