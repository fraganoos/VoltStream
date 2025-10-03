namespace ApiServices.Interfaces;
using ApiServices.DTOs.Users;
using ApiServices.Models;
using Refit;
using System.Threading.Tasks;
using VoltStream.Domain.Entities;

[Headers("accept: application/json")]
public interface ILoginApi
{
    [Post("/user/login")]
    Task<ApiResponse<Response<User>>> LoginAsync([Body] LoginRequest request);
}
