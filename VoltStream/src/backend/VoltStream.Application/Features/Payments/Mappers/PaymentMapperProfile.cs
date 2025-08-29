namespace VoltStream.Application.Features.Payments.Mappers;

using AutoMapper;
using VoltStream.Application.Features.Payments.Commands;
using VoltStream.Domain.Entities;

public class PaymentMapperProfile : Profile
{
    public PaymentMapperProfile()
    {
        CreateMap<CreatePaymentCommand, Payment>();
    }
}
