namespace ApiServices.Interfaces;

using ApiServices.Models;
using ApiServices.Models.Reqiuests;
using ApiServices.Models.Responses;
using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;

[Headers("accept: application/json")]
public interface ISaleApi
{
    [Get("/sales")]
    Task<Response<List<SaleResponse>>> GetAll();

    [Get("/sales/{id}")]
    Task<Response<SaleResponse>> GetById(long id);

    [Post("/sales")]
    Task<Response<long>> CreateAsync([Body] SaleRequest request);

    [Put("/sales")]
    Task<Response<bool>> Update([Body] SaleRequest request);

    [Delete("/sales/{id}")]
    Task<Response<bool>> Delete(long id);

    [Post("/sales/filter")]
    Task<Response<SaleResponse>> Filter(FilteringRequest request);
}
