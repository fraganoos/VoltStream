namespace VoltStream.Application.Features.Customers.DTOs;

using VoltStream.Domain.Entities;

public record AccountDto
{
    public long Id { get; set; }
    public long CustomerId { get; set; }
    public decimal BeginningSumm { get; set; }
    public decimal CurrentSumm { get; set; }
    public decimal DiscountSumm { get; set; }

    public ICollection<CustomerOperation> CustomerOperations { get; set; } = default!;
    public ICollection<DiscountOperation> DiscountOperations { get; set; } = default!;
}