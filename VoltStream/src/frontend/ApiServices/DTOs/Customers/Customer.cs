namespace ApiServices.DTOs.Customers;

public class Customer
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Description { get; set; }

    public Account Account { get; set; } = default!;
}