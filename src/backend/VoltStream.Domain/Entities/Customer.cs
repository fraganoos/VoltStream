namespace VoltStream.Domain.Entities;

public class Customer : Auditable
{
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? NormalizedEmail { get; set; }
    public string? Address { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public string? ClientType { get; set; } // Individual / Company

    public ICollection<Account> Accounts { get; set; } = default!;
    public ICollection<CustomerOperation> CustomerOperations { get; set; } = default!;
}