namespace ApiServices.Interfaces;

using ApiServices.Models;
using ApiServices.Models.Requests;
using Refit;

public interface ILoginApi
{
    [Post("/user/login")]
    Task<Response<bool>> LoginAsync(LoginRequest request);
}
