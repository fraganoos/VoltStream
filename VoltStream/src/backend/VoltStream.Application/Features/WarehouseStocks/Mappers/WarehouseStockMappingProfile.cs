namespace VoltStream.Application.Features.WarehouseStocks.Mappers;

using AutoMapper;
using VoltStream.Application.Features.WarehouseStocks.DTOs;
using VoltStream.Domain.Entities;

public class WarehouseStockMappingProfile : Profile
{
    public WarehouseStockMappingProfile()
    {
        CreateMap<WarehouseStock, WarehouseStockDto>();
        CreateMap<Product, ProductForWarehouseDto>();
    }
}
