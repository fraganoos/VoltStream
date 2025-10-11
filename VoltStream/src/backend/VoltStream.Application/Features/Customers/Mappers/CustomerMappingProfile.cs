namespace VoltStream.Application.Features.Customers.Mappers;

using AutoMapper;
using VoltStream.Application.Commons.Extensions;
using VoltStream.Application.Features.Customers.Commands;
using VoltStream.Application.Features.Customers.DTOs;
using VoltStream.Domain.Entities;

public class CustomerMappingProfile : Profile
{
    public CustomerMappingProfile()
    {
        CreateMap<Customer, CustomerDto>();

        CreateMap<CreateCustomerCommand, Customer>()
            .ForMember(dest => dest.NormalizedName, opt =>
            opt.MapFrom(src => src.Name.ToNormalized()));

        CreateMap<UpdateCustomerCommand, Customer>()
            .ForMember(dest => dest.NormalizedName, opt =>
            opt.MapFrom(src => src.Name.ToNormalized()));
    }
}
