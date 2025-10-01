namespace ApiServices.Interfaces;

using ApiServices.DTOs.Supplies;
using ApiServices.Models;
using Refit;

[Headers("accept: application/json")]
public interface IWarehouseItemsApi
{
    [Get("/warehouse-items")]
    Task<ApiResponse<Response<List<WarehouseItem>>>> GetAllWarehouseItemsAsync();
    [Get("/warehouse-items/productId={id}")]
    Task<ApiResponse<Response<List<WarehouseItem>>>> GetProductDetailsFromWarehouseAsync(long id);
    
    [Post("/warehouse-items/filter")]
    Task<ApiResponse<Response<List<WarehouseItem>>>> GetFilterFromWarehouseAsync(FilteringRequest filter);
}