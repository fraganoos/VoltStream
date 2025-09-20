namespace ApiServices.Interfaces;

using ApiServices.DTOs.Products;
using ApiServices.Models;
using Refit;

[Headers("accept: application/json")]
public interface IProductsApi
{
    [Post("/products")]
    Task<ApiResponse<Product>> CreateProductAsync([Body] Product productCreate);

    [Put("/products")]
    Task<ApiResponse<Product>> UpdateProductAsync([Body] Product productUpdate);

    [Delete("/products/{id}")]
    Task<ApiResponse<string>> DeleteProductAsync(long id);

    [Get("/products/{id}")]
    Task<ApiResponse<Product>> GetByIdProductAsync(long id);

    [Get("/products")]
    Task<ApiResponse<Response<List<Product>>>> GetAllProductsAsync();
    [Get("/products/categoryId={Id}")]
    Task<ApiResponse<Response<List<Product>>>> GetAllProductsByCategoryIdAsync(long id);
}