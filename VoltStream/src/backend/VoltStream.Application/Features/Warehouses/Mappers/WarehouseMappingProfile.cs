namespace VoltStream.Application.Features.Warehouses.Mappers;

using AutoMapper;
using VoltStream.Application.Features.Warehouses.DTOs;
using VoltStream.Domain.Entities;

public class WarehouseMappingProfile : Profile
{
    public WarehouseMappingProfile()
    {
        CreateMap<WarehouseItem, WarehouseItemDTO>();
        CreateMap<Warehouse, WarehouseDTO>();

    }
}
