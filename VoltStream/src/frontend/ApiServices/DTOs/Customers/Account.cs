namespace ApiServices.DTOs.Customers;

using System.Text.Json.Serialization;

public class Account
{
    [JsonPropertyName("id")]
    public long Id { get; set; }

    [JsonPropertyName("customerId")]
    public long CustomerId { get; set; }

    [JsonPropertyName("beginningSumm")]
    public decimal BeginningSumm { get; set; }

    [JsonPropertyName("currentSumm")]
    public decimal CurrentSumm { get; set; }
    [JsonPropertyName("discountSumm")]
    public decimal DiscountSumm { get; set; }
}