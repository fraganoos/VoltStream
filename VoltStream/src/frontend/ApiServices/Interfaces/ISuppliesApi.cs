namespace ApiServices.Interfaces;

using ApiServices.Models;
using ApiServices.Models.Requests;
using ApiServices.Models.Responses;
using Refit;

[Headers("accept: application/json")]
public interface ISuppliesApi
{
    [Post("/supplies")]
    Task<Response<long>> CreateSupplyAsync(SupplyRequest request);

    [Put("/supplies")]
    Task<Response<bool>> UpdateSupplyAsync([Body] SupplyRequest request);

    [Delete("/supplies/{id}")]
    Task<Response<bool>> DeleteSupplyAsync(long id);

    [Get("/supplies/{id}")]
    Task<Response<SupplyResponse>> GetByIdSupplyAsync(long id);

    [Get("/supplies")]
    Task<Response<List<SupplyResponse>>> GetAllSuppliesAsync();

    [Get("/supplies/by-date")]
    Task<Response<List<SupplyResponse>>> GetAllSuppliesByDateAsync([Query] DateTimeOffset date);

    [Post("/supplies/filter")]
    Task<Response<List<SupplyResponse>>> Filter(FilteringRequest request);

}