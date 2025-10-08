namespace VoltStream.Application.Features.Products.Mappers;

using AutoMapper;
using VoltStream.Application.Commons.Extensions;
using VoltStream.Application.Features.Products.Commands;
using VoltStream.Application.Features.Products.DTOs;
using VoltStream.Domain.Entities;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<CreateProductCommand, Product>()
            .ForMember(dest => dest.NormalizedName,
                opt => opt.MapFrom(src => src.Name.ToNormalized()));

        CreateMap<UpdateProductCommand, Product>()
            .ForMember(dest => dest.NormalizedName,
                opt => opt.MapFrom(src => src.Name.ToNormalized()));

        CreateMap<Product, ProductDto>();
    }
}
