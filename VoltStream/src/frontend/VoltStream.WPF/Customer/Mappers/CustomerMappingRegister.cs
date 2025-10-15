namespace VoltStream.WPF.Customer.Mappers;

using ApiServices.Models.Requests;
using ApiServices.Models.Responses;
using Mapster;
using VoltStream.WPF.Customer.ViewModels;
using VoltStream.WPF.Sales.ViewModels;

public class CustomerMappingRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Customer
        config.NewConfig<CustomerResponse, CustomerViewModel>();
        config.NewConfig<CustomerViewModel, CustomerRequest>();

        // Account
        config.NewConfig<AccountResponse, AccountViewModel>();
        config.NewConfig<AccountViewModel, AccountResponse>();

        // Customer Operation
        config.NewConfig<CustomerOperationResponse, CustomerOperationViewModel>();

        // Discount Operation
        config.NewConfig<DiscountOperationResponse, DiscountOperationViewModel>();
    }
}
