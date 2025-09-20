namespace ApiServices.Interfaces;

using ApiServices.DTOs.Supplies;
using ApiServices.Models;
using Refit;

[Headers("accept: application/json")]
public interface ISuppliesApi
{
    [Post("/supplies")]
    Task<ApiResponse<Supply>> CreateSupplyAsync(Supply supplyCreate);

    [Put("/supplies")]
    Task<ApiResponse<Supply>> UpdateSupplyAsync([Body] Supply supplyUpdate);

    [Delete("/supplies/{id}")]
    Task<ApiResponse<string>> DeleteSupplyAsync(long id);

    [Get("/supplies/{id}")]
    Task<ApiResponse<Supply>> GetByIdSupplyAsync(long id);

    [Get("/supplies")]
    Task<ApiResponse<Response<List<Supply>>>> GetAllSuppliesAsync();

    [Get("/supplies/by-date")]
    Task<ApiResponse<Response<List<Supply>>>> GetAllSuppliesByDateAsync([Query] DateTimeOffset date);

    [Post("/supplies/filter")]
    Task<ApiResponse<Response<List<Supply>>>> Filter();

}