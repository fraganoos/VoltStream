namespace ApiServices.Interfaces;

using ApiServices.DTOs.Products;
using ApiServices.Models;
using Refit;

[Headers("accept: application/json")]
public interface ICategoriesApi
{
    [Post("/categories​")]
    Task<ApiResponse<Category>> CreateAsync([Body] Category categoryCreate);

    [Put("/categories​")]
    Task<ApiResponse<Category>> UpdateAsync([Body] Category categoryUpdate);

    [Delete("/categories​/{id}")]
    Task<ApiResponse<string>> DeleteAsync(long id);

    [Get("/categories​/{id}")]
    Task<ApiResponse<Category>> GetByIdAsync(long id);

    [Get("/categories")]
    Task<ApiResponse<Response<List<Category>>>> GetAllAsync();
}
