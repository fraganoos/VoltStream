namespace VoltStream.Application.Features.DiscountOperations.DTOs;

public record DiscountOperationDto
{
    public long Id { get; set; }
    public DateTimeOffset Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public bool IsApplied { get; set; }
    public decimal Amount { get; set; }
    public long CustomerId { get; set; }
    public long SaleId { get; set; }
};