namespace VoltStream.Application.Features.Customers.DTOs;

public class CustomerDto
{
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Description { get; set; }

    public AccountDto Account { get; set; } = default!;
}