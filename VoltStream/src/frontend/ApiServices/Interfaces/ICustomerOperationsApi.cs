namespace ApiServices.Interfaces;

using ApiServices.Models;
using ApiServices.Models.Responses;
using Refit;

public interface ICustomerOperationsApi
{
    [Get("/customer-operations")]
    Task<Response<List<CustomerOperationResponse>>> GetAll();
}