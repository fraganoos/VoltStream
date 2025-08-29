namespace VoltStream.Domain.Entities;

public class Category : Auditable
{
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
}