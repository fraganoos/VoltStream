namespace VoltStream.Domain.Entities;

public class Customer : Auditable
{
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Description { get; set; }

    public ICollection<Account> Accounts { get; set; } = default!;
    public ICollection<CustomerOperation> CustomerOperations { get; set; } = default!;
    public ICollection<DiscountOperation> DiscountOperations { get; set; } = default!;
}