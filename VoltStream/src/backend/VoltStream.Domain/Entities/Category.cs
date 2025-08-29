namespace VoltStream.Domain.Entities;

public class Category : Auditable
{
    public string Name { get; set; } = string.Empty;
}