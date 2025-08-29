namespace VoltStream.Application.Features.Sales.Mappers;

using AutoMapper;
using VoltStream.Application.Features.Sales.Commands;
using VoltStream.Domain.Entities;

public class SaleMappingProfile : Profile
{
    public SaleMappingProfile()
    {
        CreateMap<CreateSaleCommand, Sale>();

        CreateMap<CreateSaleCommand, CustomerOperation>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
