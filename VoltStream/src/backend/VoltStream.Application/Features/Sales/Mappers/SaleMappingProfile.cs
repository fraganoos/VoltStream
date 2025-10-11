namespace VoltStream.Application.Features.Sales.Mappers;

using AutoMapper;
using System;
using VoltStream.Application.Features.Sales.Commands;
using VoltStream.Application.Features.Sales.DTOs;
using VoltStream.Domain.Entities;

public class SaleMappingProfile : Profile
{
    public SaleMappingProfile()
    {
        // Create/Update: incoming Date ni UTC (Offset = 0) ga normallashtirish
        CreateMap<CreateSaleCommand, Sale>()
            .ForMember(dest => dest.Date,
                opt => opt.MapFrom(src => src.Date.ToOffset(TimeSpan.Zero)));

        CreateMap<UpdateSaleCommand, Sale>()
            .ForMember(dest => dest.Date,
                opt => opt.MapFrom(src => src.Date.ToOffset(TimeSpan.Zero)));

        CreateMap<CreateSaleCommand, CustomerOperation>()
            .ForMember(dest => dest.Id, opt => opt.Ignore());

        // Entity -> DTO: Sale.Date maydoni allaqachon UTC bo'lgani uchun bevosita map qilamiz
        CreateMap<Sale, SaleDto>()
            .ForMember(dest => dest.Date,
                opt => opt.MapFrom(src => src.Date.ToOffset(TimeSpan.Zero)));

        CreateMap<SaleItem, SaleItemDto>();
        CreateMap<SaleItemCommandDto, SaleItem>();
    }
}
