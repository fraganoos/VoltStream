namespace VoltStream.Application.Features.Payments.Mappers;

using AutoMapper;
using VoltStream.Application.Features.Payments.Commands;
using VoltStream.Domain.Entities;

public class PaymentMapperProfile : Profile
{
    public PaymentMapperProfile()
    {
        CreateMap<CreatePaymentCommand, Payment>()
            .ForMember(dest => dest.PaidAt, opt => opt
                .MapFrom(src => src.PaidAt.ToUniversalTime()));
        CreateMap<UpdatePaymentCommand, Payment>()
            .ForMember(dest => dest.PaidAt, opt => opt.MapFrom(src => src.PaidAt.ToUniversalTime()));

        CreateMap<UpdatePaymentCommand, CustomerOperation>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.NetAmount));

        CreateMap<CreatePaymentCommand, CustomerOperation>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.NetAmount));
    }
}
