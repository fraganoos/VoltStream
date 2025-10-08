namespace ApiServices.DTOs.Supplies;

using System.Text.Json.Serialization;

public record ProductDto
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("categoryId")]
    public long CategoryId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("category")]
    public CategoryDto Category { get; set; } = default!;
}