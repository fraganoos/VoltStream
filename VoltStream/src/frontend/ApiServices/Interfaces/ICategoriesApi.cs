namespace ApiServices.Interfaces;

using ApiServices.DTOs.Products;
using Refit;

[Headers("accept: application/json")]
public interface ICategoriesApi
{
    [Post("/api/categories​")]
    Task<ApiResponse<Category>> CreateCategoryAsync([Body] Category categoryCreate);

    [Put("/api/categories​")]
    Task<ApiResponse<Category>> UpdateCategoryAsync([Body] Category categoryUpdate);

    [Delete("/api/categories​/{id}")]
    Task<ApiResponse<string>> DeleteCategoryAsync(long id);

    [Get("/api/categories​/{id}")]
    Task<ApiResponse<Category>> GetByIdCategoryAsync(long id);

    [Get("/api/categories​")]
    Task<ApiResponse<List<Category>>> GetAllCategoriesAsync();
}
