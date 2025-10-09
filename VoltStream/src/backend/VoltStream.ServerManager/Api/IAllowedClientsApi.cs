namespace VoltStream.ServerManager.Api;

using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;
using VoltStream.Application.Commons.Models;
using VoltStream.ServerManager.Models;

public interface IAllowedClientsApi
{
    [Get("/allowed-clients")]
    Task<Response<List<AllowedClientResponse>>> GetAllAsync();

    [Get("/allowed-clients/{id}")]
    Task<Response<AllowedClientResponse>> GetByIdAsync(long id);

    [Post("/allowed-clients")]
    Task<Response<long>> CreateAsync([Body] AllowedClientResponse command);

    [Put("/allowed-clients")]
    Task<Response<bool>> UpdateAsync([Body] AllowedClientResponse command);

    [Delete("/allowed-clients/{id}")]
    Task<Response<bool>> DeleteAsync(long id);

    [Post("/allowed-clients/filter")]
    Task<Response<List<AllowedClientResponse>>> GetFiltered([Body] FilteringRequest query);
}
