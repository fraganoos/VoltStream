namespace ApiServices.Interfaces;

using ApiServices.Models;
using ApiServices.Models.Reqiuests;
using ApiServices.Models.Responses;
using Refit;

[Headers("accept: application/json")]
public interface IProductsApi
{
    [Post("/products")]
    Task<ApiResponse<long>> CreateAsync([Body] ProductRequest request);

    [Put("/products")]
    Task<ApiResponse<bool>> UpdateAsync([Body] ProductRequest request);

    [Delete("/products/{id}")]
    Task<ApiResponse<bool>> DeleteAsync(long id);

    [Get("/products/{id}")]
    Task<ApiResponse<ProductResponse>> GetByIdAsync(long id);

    [Get("/products")]
    Task<ApiResponse<Response<List<ProductResponse>>>> GetAllAsync();
    [Get("/products/categoryId={Id}")]
    Task<ApiResponse<Response<List<ProductResponse>>>> GetAllByCategoryIdAsync(long id);
}