namespace ApiServices.Interfaces;

using ApiServices.Models;
using ApiServices.Models.Requests;
using ApiServices.Models.Responses;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface ICurrenciesApi
{
    [Post("/currencies")]
    Task<Response<long>> CreateAsync([Body] CurrencyRequest request);

    [Put("/currencies​")]
    Task<Response<bool>> UpdateAsync([Body] CurrencyRequest request);

    [Put("/currencies/all")]
    Task<Response<bool>> SaveAllAsync(List<CurrencyRequest> dtoList);

    [Delete("/currencies​/{id}")]
    Task<Response<bool>> DeleteAsync(long id);

    [Get("/currencies/{id}")]
    Task<Response<CurrencyResponse>> GetByIdAsync(long id);

    [Get("/currencies")]
    Task<Response<List<CurrencyResponse>>> GetAllAsync();
}