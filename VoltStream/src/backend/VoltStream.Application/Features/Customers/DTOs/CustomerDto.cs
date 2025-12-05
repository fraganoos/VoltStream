namespace VoltStream.Application.Features.Customers.DTOs;

using VoltStream.Application.Features.Accounts.DTOs;
using VoltStream.Application.Features.CustomerOperations.DTOs;
using VoltStream.Application.Features.DiscountOperations.DTOs;

public record CustomerDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Description { get; set; }

    public ICollection<AccountDto> Accounts { get; set; } = default!;
    public ICollection<CustomerOperationDto> CustomerOperations { get; set; } = default!;
    public ICollection<DiscountOperationDto> DiscountOperations { get; set; } = default!;
}

public record CustomerForOperationDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Description { get; set; }

    public ICollection<AccountDto> Accounts { get; set; } = default!;
}