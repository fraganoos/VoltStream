namespace VoltStream.WPF.Sales.Mappers;

using ApiServices.Models.Requests;
using ApiServices.Models.Responses;
using Mapster;
using VoltStream.WPF.Sales.ViewModels;

public class SaleMappingRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Sale
        config.NewConfig<SaleResponse, SaleViewModel>();
        config.NewConfig<SaleViewModel, SaleRequest>();

        // Sale Item
        config.NewConfig<SaleItemResponse, SaleItemViewModel>();
        config.NewConfig<SaleItemViewModel, SaleItemRequest>();

        // Warehouse stock
        config.NewConfig<WarehouseStockResponse, WarehouseStockViewModel>();
    }
}
