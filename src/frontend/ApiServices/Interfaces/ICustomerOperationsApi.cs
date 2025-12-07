namespace ApiServices.Interfaces;

using ApiServices.Models;
using ApiServices.Models.Responses;
using Refit;

public interface ICustomerOperationsApi
{
    [Get("/customer-operations")]
    Task<Response<List<CustomerOperationResponse>>> GetAll();

    [Post("/customer-operations/filter")]
    Task<Response<List<CustomerOperationResponse>>> Filter(FilteringRequest request);

    [Get("/customer-operations/by-customer-id/{customerId}")]
    Task<Response<CustomerOperationSummaryDto>> GetByCustomerId(
        long customerId,
        [Query] DateTime? beginDate = null,
        [Query] DateTime? endDate = null);

    [Get("/customer-operations/{id}")]
    Task<Response<CustomerOperationResponse>> GetById(long id);
}