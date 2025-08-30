namespace VoltStream.Application.Features.DebtKredits.Mappers;

using AutoMapper;
using VoltStream.Domain.Entities;
using VoltStream.Application.Features.DebtKredits.DTOs;
using VoltStream.Application.Features.DebtKredits.Commands;

public class DebtKreditMappingProfile : Profile
{
    public DebtKreditMappingProfile()
    {
        CreateMap<CreateDebtKreditCommand, DebtKredit>();
        CreateMap<UpdateDebtKreditCommand, DebtKredit>();
        CreateMap<DebtKredit, DebtKreditDTO>();
    }
}
