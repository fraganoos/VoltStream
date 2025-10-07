namespace ApiServices.DTOs.Sales;

using System.Text.Json.Serialization;

public class SaleItemRequest
{
    [JsonPropertyName("productId")]
    public long ProductId { get; set; }
    [JsonPropertyName("countRoll")]
    public decimal CountRoll { get; set; }
    [JsonPropertyName("quantityPerRoll")]
    public decimal QuantityPerRoll { get; set; }
    [JsonPropertyName("totalQuantity")]
    public decimal TotalQuantity { get; set; }
    [JsonPropertyName("price")]
    public decimal Price { get; set; }
    [JsonPropertyName("discountPersent")]
    public decimal DiscountPersent { get; set; }
    [JsonPropertyName("discount")]
    public decimal Discount { get; set; }
    [JsonPropertyName("totalSumm")]
    public decimal TotalSumm { get; set; }
}