namespace ApiServices.Interfaces;

using ApiServices.Models;
using ApiServices.Models.Requests;
using ApiServices.Models.Responses;
using Refit;

public interface IPaymentApi
{
    [Post("/payments")]
    Task<Response<long>> CreateAsync([Body] PaymentRequest request);

    [Post("/payments/discount")]
    Task<Response<long>> ApplyAsync([Body] ApplyDiscountRequest request);

    [Put("/payments")]
    Task<Response<bool>> UpdateAsync([Body] PaymentRequest request);

    [Get("/payments/{id}")]
    Task<Response<PaymentResponse>> GetByIdAsync(long id);

    [Get("/payments")]
    Task<Response<List<PaymentResponse>>> GetAllAsync();

    [Post("/payments/filter")]
    Task<Response<List<PaymentResponse>>> FilterAsync(FilteringRequest request);
}