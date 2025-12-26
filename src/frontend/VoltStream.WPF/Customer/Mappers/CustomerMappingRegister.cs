namespace VoltStream.WPF.Customer.Mappers;

using ApiServices.Models.Requests;
using ApiServices.Models.Responses;
using Mapster;
using VoltStream.WPF.Commons.ViewModels;

public class CustomerMappingRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CustomerResponse, CustomerViewModel>();
        config.NewConfig<CustomerViewModel, CustomerRequest>();

        config.NewConfig<AccountResponse, AccountViewModel>();
        config.NewConfig<AccountViewModel, AccountResponse>();

        config.NewConfig<CustomerOperationResponse, CustomerOperationViewModel>();
    }
}
