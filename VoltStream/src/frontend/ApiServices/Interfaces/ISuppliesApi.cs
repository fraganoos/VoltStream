namespace ApiServices.Interfaces;

using ApiServices.DTOs.Products;
using ApiServices.DTOs.Supplies;
using Refit;

[Headers("accept: application/json")]
public interface ISuppliesApi
{
    [Post("/api/Supplies​")]
    Task<ApiResponse<Supply>> CreateSupplyAsync([Body] Supply supplyCreate);

    [Put("/api/Supplies​")]
    Task<ApiResponse<Supply>> UpdateSupplyAsync([Body] Supply supplyUpdate);

    [Delete("/api/Supplies​/{id}")]
    Task<ApiResponse<string>> DeleteSupplyAsync(long id);

    [Get("/api/Supplies​/{id}")]
    Task<ApiResponse<Supply>> GetByIdSupplyAsync(long id);

    [Get("/api/Supplies")]
    Task<ApiResponse<Response<List<Supply>>>> GetAllSuppliesAsync();
}
