namespace VoltStream.Application.Features.Supplies.Mappers;

using AutoMapper;
using VoltStream.Domain.Entities;
using VoltStream.Application.Features.Supplies.Commands;

public class SupplyMappingProfile : Profile
{
    public SupplyMappingProfile()
    {
        CreateMap<CreateSupplyCommand, Supply>();
        CreateMap<UpdateSupplyCommand, Supply>();
    }
}
