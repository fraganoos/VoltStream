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

    [Get("/customer-operations/{customerId}")]
    Task<Response<CustomerOperationSummaryDto>> GetByCustomerId(
        long customerId,
        [Query] DateTime? beginDate = null,
        [Query] DateTime? endDate = null);
}