namespace ApiServices.Interfaces;

using ApiServices.Models;
using Refit;

public interface IHealthCheckApi
{
    [Get("/health")]
    Task<Response<string>> GetAsync(CancellationToken cancellationToken = default);
}