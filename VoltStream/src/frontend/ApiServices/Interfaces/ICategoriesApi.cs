namespace ApiServices.Interfaces;

using ApiServices.Models;
using ApiServices.Models.Reqiuests;
using ApiServices.Models.Responses;
using Refit;

[Headers("accept: application/json")]
public interface ICategoriesApi
{
    [Post("/categories​")]
    Task<Response<long>> CreateAsync([Body] CategoryRequest categoryCreate);

    [Put("/categories​")]
    Task<Response<bool>> UpdateAsync([Body] CategoryRequest categoryUpdate);

    [Delete("/categories​/{id}")]
    Task<Response<bool>> DeleteAsync(long id);

    [Get("/categories​/{id}")]
    Task<Response<CategoryResponse>> GetByIdAsync(long id);

    [Get("/categories")]
    Task<Response<List<CategoryResponse>>> GetAllAsync();

    [Post("/categories/filter")]
    Task<Response<List<CategoryResponse>>> Filter(FilteringRequest request);
}
