namespace ApiServices.Interfaces;

using ApiServices.Models;
using ApiServices.Models.Requests;
using ApiServices.Models.Responses;
using Refit;

[Headers("accept: application/json")]
public interface IProductsApi
{
    [Post("/products")]
    Task<Response<long>> CreateAsync([Body] ProductRequest request);

    [Put("/products")]
    Task<Response<bool>> UpdateAsync([Body] ProductRequest request);

    [Delete("/products/{id}")]
    Task<Response<bool>> DeleteAsync(long id);

    [Get("/products/{id}")]
    Task<Response<ProductResponse>> GetByIdAsync(long id);

    [Get("/products")]
    Task<Response<List<ProductResponse>>> GetAllAsync();

    [Get("/products/categoryId={Id}")]
    Task<Response<List<ProductResponse>>> GetAllByCategoryIdAsync(long id);

    [Post("/products/filter")]
    Task<Response<List<ProductResponse>>> Filter(FilteringRequest request);
}