namespace ApiServices.Interfaces;

using ApiServices.Models;
using ApiServices.Models.Reqiuests;
using ApiServices.Models.Responses;
using Refit;
using System.Threading.Tasks;

[Headers("accept: application/json")]
public interface ILoginApi
{
    [Post("/user/login")]
    Task<Response<UserResponse>> LoginAsync([Body] LoginRequest request);
}
