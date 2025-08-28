namespace VoltStream.Domain.Entities;

public class CurrencyType : Auditable
{
    public string Name { get; set; } = string.Empty;
}