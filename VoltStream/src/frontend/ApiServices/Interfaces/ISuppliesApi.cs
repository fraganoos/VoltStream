namespace ApiServices.Interfaces;

using ApiServices.DTOs.Products;
using ApiServices.DTOs.Supplies;
using Refit;

[Headers("accept: application/json")]
public interface ISuppliesApi
{
    [Post("/api/supplies")]
    Task<ApiResponse<Supply>> CreateSupplyAsync(Supply supplyCreate);

    [Put("/api/supplies")]
    Task<ApiResponse<Supply>> UpdateSupplyAsync([Body] Supply supplyUpdate);

    [Delete("/api/supplies/{id}")]
    Task<ApiResponse<string>> DeleteSupplyAsync(long id);

    [Get("/api/supplies/{id}")]
    Task<ApiResponse<Supply>> GetByIdSupplyAsync(long id);

    [Get("/api/supplies")]
    Task<ApiResponse<Response<List<Supply>>>> GetAllSuppliesAsync();
}