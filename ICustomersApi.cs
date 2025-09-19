namespace ApiServices.Interfaces;
using System;
using System.Collections.Generic;
using ApiServices.DTOs.Customers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Refit;

[Headers("accept: application/json")]
public interface ICustomersApi
{
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
