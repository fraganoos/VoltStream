namespace ApiServices.Models.Reqiuests;

public record WarehouseRequest
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
}