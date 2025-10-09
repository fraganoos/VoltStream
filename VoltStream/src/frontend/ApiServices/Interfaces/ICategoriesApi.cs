namespace ApiServices.Interfaces;

using ApiServices.Models;
using ApiServices.Models.Reqiuests;
using ApiServices.Models.Responses;
using Refit;

[Headers("accept: application/json")]
public interface ICategoriesApi
{
    [Post("/categories​")]
    Task<ApiResponse<long>> CreateAsync([Body] CategoryRequest categoryCreate);

    [Put("/categories​")]
    Task<ApiResponse<bool>> UpdateAsync([Body] CategoryRequest categoryUpdate);

    [Delete("/categories​/{id}")]
    Task<ApiResponse<bool>> DeleteAsync(long id);

    [Get("/categories​/{id}")]
    Task<ApiResponse<CategoryResponse>> GetByIdAsync(long id);

    [Get("/categories")]
    Task<ApiResponse<Response<List<CategoryResponse>>>> GetAllAsync();
}
