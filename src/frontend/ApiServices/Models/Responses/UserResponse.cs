namespace ApiServices.Models.Responses;

using ApiServices.Enums;

public record UserResponse
{
    public long Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}