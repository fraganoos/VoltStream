namespace ApiServices.Interfaces;

using Refit;
using ApiServices.DTOs.Products;
using ApiServices.DTOs.Supplies;

[Headers("accept: application/json")]
public interface IWarehouseItemsApi
{
    [Get("/api/WarehouseItems")]
    Task<ApiResponse<Response<List<WarehouseItem>>>> GetAllWarehouseItemsAsync();
}