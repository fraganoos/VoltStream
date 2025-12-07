namespace ApiServices.Models.Responses;

public record CategoryResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;


    public ICollection<ProductResponse> Products { get; set; } = default!;
}
