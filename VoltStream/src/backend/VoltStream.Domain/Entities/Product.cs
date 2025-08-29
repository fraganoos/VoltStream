namespace VoltStream.Domain.Entities;

public class Product : Auditable
{
    public long CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;

    public Category Category { get; set; } = default!;
}