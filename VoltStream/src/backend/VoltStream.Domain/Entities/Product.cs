namespace VoltStream.Domain.Entities;

public class Product : Auditable
{
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;

    public long CategoryId { get; set; }
    public Category Category { get; set; } = default!;

    public ICollection<WarehouseStock> WarehouseItems { get; set; } = default!;
}