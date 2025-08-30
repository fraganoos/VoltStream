namespace VoltStream.Application.Features.Cash.Mappers;

using AutoMapper;
using VoltStream.Domain.Entities;
using VoltStream.Application.Features.Cashes.DTOs;
using VoltStream.Application.Features.Cash.Commands;

public class CashMappingProfile : Profile
{
    public CashMappingProfile()
    {
        CreateMap<CreateCashCommand, Cash>();
        CreateMap<Cash, CashDTO>();
    }
}