namespace VoltStream.Application.Features.Warehouses.Mappers;

using AutoMapper;
using VoltStream.Application.Commons.Extensions;
using VoltStream.Application.Features.Products.DTOs;
using VoltStream.Application.Features.Warehouses.Commands;
using VoltStream.Application.Features.Warehouses.DTOs;
using VoltStream.Application.Features.WarehouseStocks.DTOs;
using VoltStream.Domain.Entities;

public class WarehouseMappingProfile : Profile
{
    public WarehouseMappingProfile()
    {
        CreateMap<WarehouseStock, WarehouseStockDto>();
        CreateMap<Warehouse, WarehouseDto>();

        CreateMap<WarehouseStock, WarehouseStockDto>();
        CreateMap<Warehouse, WarehouseDto>();
        CreateMap<Product, ProductDto>();
        CreateMap<CreateWarehouseCommand, Warehouse>()
            .ForMember(dest => dest.NormalizedName,
                opt => opt.MapFrom(src => src.Name.ToNormalized()));

        CreateMap<UpdateWarehouseCommand, Warehouse>()
            .ForMember(dest => dest.NormalizedName,
                opt => opt.MapFrom(src => src.Name.ToNormalized()));
    }
}
