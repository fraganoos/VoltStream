namespace ApiServices.Models.Responses;

public class ProductResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public CategoryResponse Category { get; set; } = default!;
    public ICollection<WarehouseStockResponse> Stocks { get; set; } = default!;
}