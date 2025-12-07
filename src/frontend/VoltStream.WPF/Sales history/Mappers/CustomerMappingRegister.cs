namespace VoltStream.WPF.Products.Mappers;

using ApiServices.Models.Responses;
using Mapster;

public class CustomerMappingRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CustomerResponse, CustomerResponse>();
    }
}
