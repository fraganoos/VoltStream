namespace ApiServices.DTOs.Products;

using System.Text.Json.Serialization;

public class Product
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("categoryId")]
    public long CategoryId { get; set; }
}
