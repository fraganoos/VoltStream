namespace ApiServices.Models.Responses;

public class WarehouseResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<WarehouseStockResponse> Stocks { get; set; } = default!;
}