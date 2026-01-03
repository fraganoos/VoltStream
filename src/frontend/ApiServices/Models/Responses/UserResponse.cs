namespace ApiServices.Models.Responses;

using ApiServices.Enums;

public record UserResponse
{
    public long Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}