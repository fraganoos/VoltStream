namespace VoltStream.Application.Features.Categories.DTOs;

using VoltStream.Application.Features.Products.DTOs;

public class CategoryDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public ICollection<ProductDto> Products { get; set; } = default!;
}
