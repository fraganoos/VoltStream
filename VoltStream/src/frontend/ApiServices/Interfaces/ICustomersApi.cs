namespace ApiServices.Interfaces;

using ApiServices.DTOs.Customers;
using ApiServices.DTOs.Products;
using Refit;

public interface ICustomersApi
{
    [Post("/api/customers​")]
    Task<ApiResponse<Customer>> CreateAsync([Body] Customer CustomersCreate);

    [Put("/api/customers​")]
    Task<ApiResponse<Customer>> UpdateAsync([Body] Customer CustomersUpdate);

    [Delete("/api/customers​/{id}")]
    Task<ApiResponse<string>> DeleteAsync(long id);

    [Get("/api/customers​/{id}")]
    Task<ApiResponse<Customer>> GetByIdAsync(long id);

    [Get("/api/customers")]
    Task<ApiResponse<Response<List<Customer>>>> GetAllAsync();
}