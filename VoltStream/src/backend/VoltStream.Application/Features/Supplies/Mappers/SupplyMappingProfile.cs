namespace VoltStream.Application.Features.Supplies.Mappers;

using AutoMapper;
using VoltStream.Application.Features.Supplies.Commands;
using VoltStream.Application.Features.Supplies.DTOs;
using VoltStream.Domain.Entities;

public class SupplyMappingProfile : Profile
{
    public SupplyMappingProfile()
    {
        CreateMap<CreateSupplyCommand, Supply>();
        CreateMap<UpdateSupplyCommand, Supply>();
        CreateMap<CreateSupplyCommand, WarehouseStock>();
        CreateMap<Supply, SupplyDto>();
    }
}
