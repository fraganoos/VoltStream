namespace VoltStream.Application.Features.CustomerOperations.Mappers;

using AutoMapper;
using VoltStream.Application.Features.CustomerOperations.DTOs;
using VoltStream.Domain.Entities;

public class CustomerOperationMappingProfile : Profile
{
    public CustomerOperationMappingProfile()
    {
        CreateMap<CustomerOperation, CustomerOperationDto>();
        CreateMap<CustomerOperation, CustomerOperationForPaymentDto>();
        CreateMap<CustomerOperation, CustomerOperationForSaleDto>();
    }
}
