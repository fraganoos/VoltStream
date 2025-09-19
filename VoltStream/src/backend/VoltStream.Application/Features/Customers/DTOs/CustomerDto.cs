namespace VoltStream.Application.Features.Customers.DTOs;

using VoltStream.Domain.Entities;

public record CustomerDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Description { get; set; }

    public AccountDto Account { get; set; } = default!;
    public ICollection<CustomerOperation> CustomerOperations { get; set; } = default!;
    public ICollection<DiscountOperation> DiscountOperations { get; set; } = default!;
}