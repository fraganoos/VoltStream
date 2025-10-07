namespace VoltStream.Application.Features.DiscountOperations.DTOs;

using VoltStream.Domain.Entities;

public record DiscountOperationDto
{
    public long Id { get; set; }
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public bool IsApplied { get; set; }
    public decimal DiscountAmount { get; set; }

    public long CustomerId { get; set; }
    public Customer Customer { get; set; } = default!;
}
