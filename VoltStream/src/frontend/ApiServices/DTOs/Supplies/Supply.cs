namespace ApiServices.DTOs.Supplies;

using ApiServices.DTOs.Products;
using System.Text.Json.Serialization;

public record Supply
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("operationDate")]
    public DateTimeOffset OperationDate { get; set; }
    
    [JsonPropertyName("categoryId")]
    public long CategoryId { get; set; }
    
    [JsonPropertyName("productId")]
    public long ProductId { get; set; }
    
    [JsonPropertyName("countRoll")]
    public decimal CountRoll { get; set; }
    
    [JsonPropertyName("quantityPerRoll")]
    public decimal QuantityPerRoll { get; set; }
    
    [JsonPropertyName("totalQuantity")]
    public decimal TotalQuantity { get; set; }
    
    [JsonPropertyName("productName")]
    public string ProductName { get; set; } = string.Empty;
    
    [JsonPropertyName("categoryName")]
    public string CategoryName { get; set; } = string.Empty;
    
    [JsonPropertyName("price")]
    public decimal Price { get; set; }
    
    [JsonPropertyName("discountPercent")]
    public decimal DiscountPercent { get; set; }
}