namespace ApiServices.Interfaces;

using Refit;
using ApiServices.DTOs.Products;
using ApiServices.DTOs.Supplies;

[Headers("accept: application/json")]
public interface IWarehouseItemsApi
{
    [Get("/api/warehouse-items")]
    Task<ApiResponse<Response<List<WarehouseItem>>>> GetAllWarehouseItemsAsync();
}