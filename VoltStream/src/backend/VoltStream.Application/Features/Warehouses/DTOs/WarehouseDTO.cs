namespace VoltStream.Application.Features.Warehouses.DTOs;

public record WarehouseDTO
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
}