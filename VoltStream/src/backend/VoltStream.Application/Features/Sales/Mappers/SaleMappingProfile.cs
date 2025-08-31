namespace VoltStream.Application.Features.Sales.Mappers;

using AutoMapper;
using VoltStream.Application.Features.Sales.Commands;
using VoltStream.Application.Features.Sales.DTOs;
using VoltStream.Domain.Entities;

public class SaleMappingProfile : Profile
{
    public SaleMappingProfile()
    {
        CreateMap<CreateSaleCommand, Sale>();
        CreateMap<UpdateSaleCommand, Sale>();

        CreateMap<CreateSaleCommand, CustomerOperation>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        CreateMap<Sale, SalesDto>();
        CreateMap<SaleItem, SalesItemDto>();
    }
}
