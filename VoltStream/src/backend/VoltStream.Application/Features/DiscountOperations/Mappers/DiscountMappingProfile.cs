namespace VoltStream.Application.Features.DiscountOperations.Mappers;

using AutoMapper;
using VoltStream.Application.Features.DiscountOperations.DTOs;
using VoltStream.Domain.Entities;

public class DiscountMappingProfile : Profile
{
    public DiscountMappingProfile()
    {
        CreateMap<DiscountOperation, DiscountOperationDto>();
        CreateMap<DiscountOperationCommandDto, DiscountOperation>();
    }
}
