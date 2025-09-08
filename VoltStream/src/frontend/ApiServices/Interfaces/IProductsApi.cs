namespace ApiServices.Interfaces;

using ApiServices.DTOs.Products;
using Refit;

[Headers("accept: application/json")]
public interface IProductsApi
{
    [Post("/api/products")]
    Task<ApiResponse<Product>> CreateProductAsync([Body] Product productCreate);

    [Put("/api/products")]
    Task<ApiResponse<Product>> UpdateProductAsync([Body] Product productUpdate);

    [Delete("/api/products/{id}")]
    Task<ApiResponse<string>> DeleteProductAsync(long id);

    [Get("/api/products/{id}")]
    Task<ApiResponse<Product>> GetByIdProductAsync(long id);

    [Get("/api/products")]
    Task<ApiResponse<List<Product>>> GetAllProductsAsync();
}
