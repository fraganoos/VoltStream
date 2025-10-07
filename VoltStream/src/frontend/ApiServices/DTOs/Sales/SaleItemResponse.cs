namespace ApiServices.DTOs.Sales;

using ApiServices.DTOs.Products;

public class SaleItemResponse
{
    public long Id { get; set; }
    public long SaleId { get; set; }
    public long ProductId { get; set; }
    public decimal CountRoll { get; set; }
    public decimal QuantityPerRoll { get; set; }
    public decimal TotalQuantity { get; set; }
    public decimal Price { get; set; }
    public decimal DiscountPersent { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalSumm { get; set; }

    public Product Product { get; set; } = default!;
}