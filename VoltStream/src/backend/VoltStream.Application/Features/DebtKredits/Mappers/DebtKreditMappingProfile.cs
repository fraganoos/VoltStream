namespace VoltStream.Application.Features.DebtKredits.Mappers;

using AutoMapper;
using VoltStream.Application.Features.DebtKredits.Commands;
using VoltStream.Application.Features.DebtKredits.DTOs;
using VoltStream.Domain.Entities;

public class DebtKreditMappingProfile : Profile
{
    public DebtKreditMappingProfile()
    {
        CreateMap<CreateDebtKreditCommand, DebtKredit>();
        CreateMap<UpdateDebtKreditCommand, DebtKredit>();
        CreateMap<DebtKredit, DebtKreditDTO>();
    }
}
