namespace VoltStream.Application.Features.Products.Mappers;

using AutoMapper;
using VoltStream.Application.Features.Products.Commands;
using VoltStream.Application.Features.Products.DTOs;
using VoltStream.Domain.Entities;

public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<CreateProductCommand, Product>();
        CreateMap<Product, ProductDTO>();
    }
}
