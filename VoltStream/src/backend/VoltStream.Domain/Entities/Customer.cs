namespace VoltStream.Domain.Entities;

public class Customer : Auditable
{
    public string Name { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Address { get; set; }
    public string? Description { get; set; }
}