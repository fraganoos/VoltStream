namespace ApiServices.Interfaces;

using ApiServices.DTOs.Customers;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

[Headers("accept: application/json")]
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
    Task<ApiResponse<List<Customer>>> GetAllCustomersAsync();

    [Get("/api/customers/{id}")]
    Task<ApiResponse<Customer>> GetCustomerByIdAsync(long id);

    [Post("/api/customers")]
    Task<ApiResponse<Customer>> CreateCustomerAsync([Body] Customer customerCreate);

    [Put("/api/customers")]
    Task<ApiResponse<Customer>> UpdateCustomerAsync([Body] Customer customerUpdate);

    [Delete("/api/customers/{id}")]
    Task<ApiResponse<string>> DeleteCustomerAsync(long id);
}
