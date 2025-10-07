namespace ApiServices.Interfaces;

using ApiServices.DTOs.Products;
using ApiServices.Models;
using Refit;

[Headers("accept: application/json")]
public interface IProductsApi
{
    [Post("/products")]
    Task<ApiResponse<Product>> CreateAsync([Body] Product productCreate);

    [Put("/products")]
    Task<ApiResponse<Product>> UpdateAsync([Body] Product productUpdate);

    [Delete("/products/{id}")]
    Task<ApiResponse<string>> DeleteAsync(long id);

    [Get("/products/{id}")]
    Task<ApiResponse<Product>> GetByIdAsync(long id);

    [Get("/products")]
    Task<ApiResponse<Response<List<Product>>>> GetAllAsync();
    [Get("/products/categoryId={Id}")]
    Task<ApiResponse<Response<List<Product>>>> GetAllByCategoryIdAsync(long id);
}