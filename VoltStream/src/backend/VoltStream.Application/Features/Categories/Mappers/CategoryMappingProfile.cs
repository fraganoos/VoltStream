using AutoMapper;
using VoltStream.Domain.Entities;
using VoltStream.Application.Features.Categories.DTOs;
using VoltStream.Application.Features.Categories.Commands;


namespace VoltStream.Application.Features.Categories.Mappers;

public class CategoryMappingProfile : Profile
{
    public CategoryMappingProfile()
    {
        CreateMap<CreateCategoryCommand, Category>();
        CreateMap<Category, CategoryDto>();
    }
}
