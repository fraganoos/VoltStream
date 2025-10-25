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


        // --- Payment ---
        // --- Payment ---
        config.NewConfig<PaymentResponse, PaymentViewModel>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.PaidAt, src => src.PaidAt.LocalDateTime)
            // faqat Amount uchun to‘g‘rilash:
            .Map(dest => dest.Amount, src =>
                src.Amount != default(decimal) ? src.Amount : src.NetAmount)
            .Map(dest => dest.ExchangeRate, src => src.ExchangeRate)
            .Map(dest => dest.NetAmount, src => src.NetAmount)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.Currency, src => src.Currency)
            .Map(dest => dest.CurrencyId, src => src.Currency.Id)
            .Map(dest => dest.Customer, src => src.Customer)
            .Map(dest => dest.CustomerId, src => src.Customer.Id)
            .IgnoreNonMapped(true); // UI uchun faqat kerakli propertylar ishlaydi


        // --- PaymentViewModel -> PaymentRequest (yuborish uchun) ---
        config.NewConfig<PaymentViewModel, PaymentRequest>()
            .Map(dest => dest.Id, src => src.Id)
            .Map(dest => dest.PaidAt, src => src.PaidAt)
            .Map(dest => dest.Amount, src => src.Amount)
            .Map(dest => dest.ExchangeRate, src => src.ExchangeRate)
            .Map(dest => dest.NetAmount, src => src.NetAmount)
            .Map(dest => dest.Description, src => src.Description)
            .Map(dest => dest.CurrencyId, src => src.CurrencyId)
            .Map(dest => dest.CustomerId, src => src.CustomerId);
    }
}
