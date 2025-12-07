namespace VoltStream.WPF.Sales.Mappers;

using ApiServices.Models.Requests;
using ApiServices.Models.Responses;
using Mapster;
using VoltStream.WPF.Sales.ViewModels;
using VoltStream.WPF.Turnovers.Models;

public class SaleMappingRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Sale
        config.NewConfig<SaleResponse, SaleViewModel>()
            .Map(dest => dest.Date, src => src.Date.LocalDateTime);

        // Sale Item
        config.NewConfig<SaleItemResponse, SaleItemViewModel>();
        config.NewConfig<SaleItemViewModel, SaleItemRequest>();

        // Warehouse stock
        config.NewConfig<WarehouseStockResponse, WarehouseStockViewModel>();

        config.NewConfig<SaleResponse, SalePageViewModel>()
            .Map(dest => dest.Date, src => src.Date.LocalDateTime);
    }
}
