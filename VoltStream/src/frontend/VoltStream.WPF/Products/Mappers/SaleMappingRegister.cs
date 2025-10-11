namespace VoltStream.WPF.Products.Mappers;

using ApiServices.DTOs.Supplies;
using ApiServices.Models.Responses;
using Mapster;

public class ProductMappingRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CategoryResponse, Category>();

        config.NewConfig<ProductResponse, Product>();
    }
}
