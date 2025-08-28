namespace VoltStream.Domain.Entities;

public class Product : Auditable
{
    public long CategoryId { get; set; }
    public string Name { get; set; } = null!;
}