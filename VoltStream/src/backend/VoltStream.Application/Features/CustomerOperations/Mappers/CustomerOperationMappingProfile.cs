namespace VoltStream.Application.Features.CustomerOperations.Mappers;

using AutoMapper;
using VoltStream.Application.Features.CustomerOperations.Queries;
using VoltStream.Domain.Entities;

public class CustomerOperationMappingProfile : Profile
{
    public CustomerOperationMappingProfile()
    {
        CreateMap<GetAllCustomerOperationsQuery, CustomerOperation>();
    }
}
