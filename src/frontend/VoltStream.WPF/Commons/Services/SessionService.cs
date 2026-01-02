namespace VoltStream.WPF.Commons.Services;

using ApiServices.Enums;
using ApiServices.Models.Responses;

public interface ISessionService
{
    UserResponse? CurrentUser { get; set; }
    string? Token => CurrentUser?.Token;
    bool IsAdmin => CurrentUser?.Role == UserRole.Admin;
}

public class SessionService : ISessionService
{
    public UserResponse? CurrentUser { get; set; }
}
