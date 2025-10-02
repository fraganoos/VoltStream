namespace VoltStream.ServerManager.Api;

using Refit;
using System.Collections.Generic;
using System.Threading.Tasks;
using VoltStream.Application.Commons.Models;
using VoltStream.ServerManager.Models;

public interface IAllowedClientsApi
{
    [Get("/allowed-clients")]
    Task<Response<List<AllowedClientDto>>> GetAllAsync();

    [Get("/allowed-clients/{id}")]
    Task<Response<AllowedClientDto>> GetByIdAsync(long id);

    [Post("/allowed-clients")]
    Task<Response<long>> CreateAsync([Body] AllowedClientDto command);

    [Put("/allowed-clients")]
    Task<Response<bool>> UpdateAsync([Body] AllowedClientDto command);

    [Delete("/allowed-clients/{id}")]
    Task<Response<bool>> DeleteAsync(long id);

    [Post("/allowed-clients/filter")]
    Task<Response<List<AllowedClientDto>>> GetFiltered([Body] FilteringRequest query);
}
