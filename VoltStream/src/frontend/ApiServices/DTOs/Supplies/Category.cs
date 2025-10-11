namespace ApiServices.DTOs.Supplies;

using System.Text.Json.Serialization;

public class Category
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

}
