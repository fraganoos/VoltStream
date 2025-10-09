namespace ApiServices.DTOs.Supplies;

using System.Text.Json.Serialization;

public record WarehouseItem
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("productId")]
    public long ProductId { get; set; }

    [JsonPropertyName("countRoll")]
    public decimal CountRoll { get; set; }  // rulon soni

    [JsonPropertyName("quantityPerRoll")]
    public decimal QuantityPerRoll { get; set; }  //rulon uzunligi

    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    [JsonPropertyName("discountPercent")]
    public decimal DiscountPercent { get; set; }

    [JsonPropertyName("totalQuantity")]
    public decimal TotalQuantity { get; set; } // jami uzunlik

    [JsonPropertyName("product")]
    public ProductDto Product { get; set; } = default!;
}
