namespace VoltStream.Application.Features.Cashes.DTOs;
public record CashDTO
{
    public long Id { get; set; }
    public decimal UzsBalance { get; set; }
    public decimal UsdBalance { get; set; }
    public decimal Kurs { get; set; }
}
