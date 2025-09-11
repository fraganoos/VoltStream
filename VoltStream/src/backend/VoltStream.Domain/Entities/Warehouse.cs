namespace VoltStream.Domain.Entities;

public class Warehouse : Auditable
{
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
    public ICollection<WarehouseItem> Items { get; set; } = default!;
}