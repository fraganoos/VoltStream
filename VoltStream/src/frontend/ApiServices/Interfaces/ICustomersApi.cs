namespace ApiServices.Interfaces;

using ApiServices.DTOs.Customers;
using ApiServices.Models;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

[Headers("accept: application/json")]
public interface ICustomersApi
{
    [Post("/customers")]
    Task<ApiResponse<Response<long>>> CreateAsync([Body] Customer customer);

    [Put("/customers​")]
    Task<ApiResponse<Response<Customer>>> UpdateAsync([Body] Customer customer);

    [Delete("/customers​/{id}")]
    Task<ApiResponse<Response<bool>>> DeleteAsync(long id);

    [Get("/customers​/{id}")]
    Task<ApiResponse<Response<Customer>>> GetByIdAsync(long id);

    [Get("/customers")]
    Task<ApiResponse<Response<List<Customer>>>> GetAllCustomersAsync();

    [Get("/customers/{id}")]
    Task<ApiResponse<Response<Customer>>> GetCustomerByIdAsync(long id);

    [Post("/customers/filter")]
    Task<ApiResponse<Response<List<Customer>>>> Filter(FilteringRequest request);
}
