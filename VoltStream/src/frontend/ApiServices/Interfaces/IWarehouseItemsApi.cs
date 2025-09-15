namespace ApiServices.Interfaces;

using ApiServices.DTOs.Products;
using ApiServices.DTOs.Supplies;
using Refit;

[Headers("accept: application/json")]
public interface IWarehouseItemsApi
{
    [Get("/api/warehouse-items")]
    Task<ApiResponse<Response<List<WarehouseItem>>>> GetAllWarehouseItemsAsync();
}