namespace VoltStream.Application.Features.Accounts.Mappers;

using AutoMapper;
using VoltStream.Application.Features.Accounts.DTOs;
using VoltStream.Domain.Entities;

public class AccountMappingProfile : Profile
{
    public AccountMappingProfile()
    {
        CreateMap<Account, AccountDto>().ReverseMap();
        CreateMap<AccountCommandDto, Account>();
    }
}
