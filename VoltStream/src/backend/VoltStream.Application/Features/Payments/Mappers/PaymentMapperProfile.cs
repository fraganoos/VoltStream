namespace VoltStream.Application.Features.Payments.Mappers;

using AutoMapper;
using VoltStream.Application.Features.Payments.Commands;
using VoltStream.Application.Features.Payments.DTOs;
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

        // NEW: Payment → PaymentDto (xatoni tuzatadi)
        CreateMap<Payment, PaymentDto>()
            .ForMember(dest => dest.PaidAt, opt => opt.MapFrom(src => src.PaidAt))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
            .ForMember(dest => dest.ExchangeRate, opt => opt.MapFrom(src => src.ExchangeRate))
            .ForMember(dest => dest.NetAmount, opt => opt.MapFrom(src => src.NetAmount))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Currency))
            .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => src.Customer));
    }
}
