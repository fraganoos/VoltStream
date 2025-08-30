namespace VoltStream.Application.Features.Cash.Mappers;

using AutoMapper;
using VoltStream.Application.Features.Cash.Commands;
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