namespace VoltStream.Application.Features.Products.DTOs;

public class ProductDto
{
    public long Id { get; set; }
    public long CategoryId { get; set; }
    public string Name { get; set; } = null!;
}