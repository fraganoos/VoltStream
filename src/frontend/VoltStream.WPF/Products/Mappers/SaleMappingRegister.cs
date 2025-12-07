namespace VoltStream.WPF.Products.Mappers;

using ApiServices.Models.Responses;
using Mapster;

public class ProductMappingRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CategoryResponse, CategoryResponse>();
        config.NewConfig<ProductResponse, ProductResponse>();
    }
}
