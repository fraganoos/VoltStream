namespace VoltStream.Domain.Entities;

public class Cash : Auditable
{
    public decimal UzsBalance { get; set; }
    public decimal UsdBalance { get; set; }
    public decimal Kurs { get; set; }
}