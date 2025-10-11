namespace VoltStream.Application.Features.Categories.Mappers;

using AutoMapper;
using VoltStream.Application.Commons.Extensions;
using VoltStream.Application.Features.Categories.Commands;
using VoltStream.Application.Features.Categories.DTOs;
using VoltStream.Domain.Entities;

public class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        CreateMap<CreateCategoryCommand, Category>()
            .ForMember(dest => dest.NormalizedName,
            opt => opt.MapFrom(src => src.Name.ToNormalized()));

        CreateMap<UpdateCategoryCommand, Category>()
            .ForMember(dest => dest.NormalizedName,
            opt => opt.MapFrom(src => src.Name.ToNormalized()));

        CreateMap<Category, CategoryDto>();

        CreateMap<Category, CategoryForProductDto>();

    }
}
