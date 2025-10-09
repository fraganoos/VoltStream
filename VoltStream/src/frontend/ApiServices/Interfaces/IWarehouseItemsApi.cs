namespace ApiServices.Interfaces;

using ApiServices.Models;
using ApiServices.Models.Responses;
using Refit;

[Headers("accept: application/json")]
public interface IWarehouseItemsApi
{
    [Get("/warehouse-items")]
    Task<ApiResponse<Response<List<WarehouseStockResponse>>>> GetAllWarehouseItemsAsync();
    [Get("/warehouse-items/productId={id}")]
    Task<ApiResponse<Response<List<WarehouseStockResponse>>>> GetProductDetailsFromWarehouseAsync(long id);

    [Post("/warehouse-items/filter")]
    Task<ApiResponse<Response<List<WarehouseStockResponse>>>> GetFilterFromWarehouseAsync(FilteringRequest filter);
}