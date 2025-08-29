namespace VoltStream.Application.Features.Payments.Mappers;

using AutoMapper;
using VoltStream.Application.Features.Payments.Commands;
using VoltStream.Domain.Entities;

public class PaymentMapperProfile : Profile
{
    public PaymentMapperProfile()
    {
        CreateMap<CreatePaymentCommand, Payment>();
        CreateMap<UpdatePaymentCommand, Payment>();

        CreateMap<UpdatePaymentCommand, CustomerOperation>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Summa, opt => opt.MapFrom(src => src.DefaultSumm));

        CreateMap<CreatePaymentCommand, CustomerOperation>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Summa, opt => opt.MapFrom(src => src.DefaultSumm));
    }
}
