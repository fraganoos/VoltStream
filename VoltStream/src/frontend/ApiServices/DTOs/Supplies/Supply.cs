using ApiServices.DTOs.Products;

namespace ApiServices.DTOs.Supplies;
public record Supply
{
    public long Id { get; set; }
    public DateTimeOffset OperationDate { get; set; }
    public long CategoryId { get; set; }
    public long ProductId { get; set; }
    public decimal CountRoll { get; set; }
    public decimal QuantityPerRoll { get; set; }
    public decimal TotalQuantity { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public Product Product { get; set; } = default!;
}