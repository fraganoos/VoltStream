namespace VoltStream.Application.Features.Monitoring.Mappers;

using AutoMapper;
using VoltStream.Application.Commons.Extensions;
using VoltStream.Application.Features.Monitoring.Commands;
using VoltStream.Domain.Entities;

public class CustomerMappingProfile : Profile
{
    public CustomerMappingProfile()
    {
        CreateMap<CreateAllowedClientCommand, AllowedClient>()
            .ForMember(dest => dest.NormalizedName, opt =>
            opt.MapFrom(src => src.DeviceName!.ToNormalized()));

        CreateMap<UpdateAllowedClientCommand, AllowedClient>()
            .ForMember(dest => dest.NormalizedName, opt =>
            opt.MapFrom(src => src.DeviceName!.ToNormalized()));
    }
}
