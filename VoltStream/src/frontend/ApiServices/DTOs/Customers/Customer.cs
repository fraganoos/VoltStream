namespace ApiServices.DTOs.Customers;
using System.Text.Json.Serialization;

public record Customer
{
    [JsonPropertyName("id")]
    public int Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("phone")]
    public string? Phone { get; set; }
    
    [JsonPropertyName("address")]
    public string? Address { get; set; }
    public string? Description { get; set; }
    
    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
