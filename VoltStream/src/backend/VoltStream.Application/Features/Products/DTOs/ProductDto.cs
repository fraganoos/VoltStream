namespace VoltStream.Application.Features.Products.DTOs;

public record ProductDTO
{
    public long Id { get; set; }
    public long CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
}