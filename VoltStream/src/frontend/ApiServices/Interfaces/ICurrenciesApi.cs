namespace ApiServices.Interfaces;

using ApiServices.Models;
using ApiServices.Models.Requests;
using ApiServices.Models.Responses;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

[Headers("accept: application/json")]
public interface ICurrenciesApi
{
    [Post("/Currencies")]
    Task<Response<long>> CreateAsync([Body] CurrencyRequest request);

    [Put("/Currencies​")]
    Task<Response<bool>> UpdateAsync([Body] CurrencyRequest request);

    [Delete("/Currencies​/{id}")]
    Task<Response<bool>> DeleteAsync(long id);

    [Get("/Currencies/{id}")]
    Task<Response<CurrencyResponse>> GetByIdAsync(long id);

    [Get("/Currencies")]
    Task<Response<List<CurrencyResponse>>> GetAllAsync();
}