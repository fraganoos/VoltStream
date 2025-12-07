namespace ApiServices.Interfaces;

using ApiServices.Models;
using ApiServices.Models.Requests;
using ApiServices.Models.Responses;
using Refit;

public interface IRegisterApi
{
    [Post("/user/register")]
    Task<Response<UserResponse>> RegisterAsync(RegisterRequest request);
}