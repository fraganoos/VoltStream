namespace ApiServices.Interfaces;

using ApiServices.Models;
using ApiServices.Models.Requests;
using Refit;

public interface IDiscountsApi
{
    [Post("/discounts")]
    Task<Response<long>> ApplyAsync(ApplyDiscountRequest command);
}