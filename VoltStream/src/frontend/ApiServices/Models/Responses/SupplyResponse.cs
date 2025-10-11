namespace ApiServices.Models.Responses;

public record SupplyResponse
{
    public long Id { get; set; }
    public DateTimeOffset Date { get; set; }
    public decimal RollCount { get; set; }
    public decimal LengthPerRoll { get; set; }
    public decimal TotalLength { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public long ProductId { get; set; }
    public ProductResponse Product { get; set; } = default!;
}