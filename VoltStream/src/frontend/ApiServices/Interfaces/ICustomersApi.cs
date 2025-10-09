namespace ApiServices.Interfaces;

using ApiServices.Models;
using ApiServices.Models.Reqiuests;
using ApiServices.Models.Responses;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

[Headers("accept: application/json")]
public interface ICustomersApi
{
    [Post("/customers")]
    Task<ApiResponse<Response<long>>> CreateAsync([Body] CustomerRequest request);

    [Put("/customers​")]
    Task<ApiResponse<Response<bool>>> UpdateAsync([Body] CustomerRequest request);

    [Delete("/customers​/{id}")]
    Task<ApiResponse<Response<bool>>> DeleteAsync(long id);

    [Get("/customers/{id}")]
    Task<ApiResponse<Response<CustomerResponse>>> GetByIdAsync(long id);

    [Get("/customers")]
    Task<ApiResponse<Response<List<CustomerResponse>>>> GetAllAsync();

    [Post("/customers/filter")]
    Task<ApiResponse<Response<List<CustomerResponse>>>> Filter(FilteringRequest request);
}
