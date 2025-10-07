namespace ApiServices.DTOs.Sales;

using System.Text.Json.Serialization;

public record SaleRequest
{
    [JsonPropertyName("operationDate")]
    public DateTimeOffset OperationDate { get; set; }
    [JsonPropertyName("customerId")]
    public long CustomerId { get; set; }
    [JsonPropertyName("summa")]
    public decimal Summa { get; set; }
    [JsonPropertyName("customerOperationId")]
    public long CustomerOperationId { get; set; }
    [JsonPropertyName("discount")]
    public decimal? Discount { get; set; }
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
    [JsonPropertyName("saleItems")]
    public List<SaleItemRequest> SaleItems { get; set; } = default!;
}