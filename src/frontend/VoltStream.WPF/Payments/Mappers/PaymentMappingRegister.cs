namespace VoltStream.WPF.Payments.Mappers;

using ApiServices.Models.Requests;
using ApiServices.Models.Responses;
using Mapster;
using VoltStream.WPF.Commons.ViewModels;

public class PaymentMappingRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<CurrencyResponse, CurrencyViewModel>();
        config.NewConfig<CurrencyViewModel, CurrencyRequest>();

        config.NewConfig<CustomerResponse, CustomerViewModel>();
        config.NewConfig<CustomerViewModel, CustomerRequest>();

        config.NewConfig<PaymentResponse, PaymentViewModel>()
            .Map(dest => dest.PaidAt, src => src.PaidAt.LocalDateTime);

        config.NewConfig<PaymentViewModel, PaymentRequest>()
            .Map(dest => dest.CurrencyId, src => src.Currency.Id)
            .Map(dest => dest.CustomerId, src => src.Customer.Id);
    }
}
