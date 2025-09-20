namespace ApiServices.Interfaces;

using ApiServices.DTOs.Products;
using ApiServices.Models;
using Refit;

[Headers("accept: application/json")]
public interface ICategoriesApi
{
    [Post("/categories​")]
    Task<ApiResponse<Category>> CreateCategoryAsync([Body] Category categoryCreate);

    [Put("/categories​")]
    Task<ApiResponse<Category>> UpdateCategoryAsync([Body] Category categoryUpdate);

    [Delete("/categories​/{id}")]
    Task<ApiResponse<string>> DeleteCategoryAsync(long id);

    [Get("/categories​/{id}")]
    Task<ApiResponse<Category>> GetByIdCategoryAsync(long id);

    [Get("/categories")]
    Task<ApiResponse<Response<List<Category>>>> GetAllCategoriesAsync();
}
