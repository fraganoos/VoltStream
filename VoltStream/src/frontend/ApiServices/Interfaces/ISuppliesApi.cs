namespace ApiServices.Interfaces;

using ApiServices.Models;
using ApiServices.Models.Reqiuests;
using ApiServices.Models.Responses;
using Refit;

[Headers("accept: application/json")]
public interface ISuppliesApi
{
    [Post("/supplies")]
    Task<ApiResponse<long>> CreateSupplyAsync(SupplyRequest request);

    [Put("/supplies")]
    Task<ApiResponse<bool>> UpdateSupplyAsync([Body] SupplyRequest request);

    [Delete("/supplies/{id}")]
    Task<ApiResponse<bool>> DeleteSupplyAsync(long id);

    [Get("/supplies/{id}")]
    Task<ApiResponse<SupplyResponse>> GetByIdSupplyAsync(long id);

    [Get("/supplies")]
    Task<ApiResponse<Response<List<SupplyResponse>>>> GetAllSuppliesAsync();

    [Get("/supplies/by-date")]
    Task<ApiResponse<Response<List<SupplyResponse>>>> GetAllSuppliesByDateAsync([Query] DateTimeOffset date);

    [Post("/supplies/filter")]
    Task<ApiResponse<Response<List<SupplyResponse>>>> Filter();

}