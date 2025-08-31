namespace VoltStream.Application.Features.Cashes.Mappers;

using AutoMapper;
using VoltStream.Application.Features.Cashes.Commands;
using VoltStream.Application.Features.Cashes.DTOs;
using VoltStream.Domain.Entities;

public class CashMappingProfile : Profile
{
    public CashMappingProfile()
    {
        CreateMap<CreateCashCommand, Cash>();
        CreateMap<Cash, CashDTO>();
    }
}