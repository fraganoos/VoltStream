namespace VoltStream.WPF.Sales.Mappers;

using ApiServices.Models.Reqiuests;
using ApiServices.Models.Responses;
using Mapster;
using VoltStream.WPF.Sales.ViewModels;

public class SupplyMappingRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ProductResponse, ProductViewModel>();
        config.NewConfig<ProductViewModel, ProductRequest>();

        config.NewConfig<CategoryResponse, CategoryViewModel>();
        config.NewConfig<CategoryViewModel, CategoryRequest>();
    }
}
