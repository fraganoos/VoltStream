namespace VoltStream.Application.Features.Users.DTOs;

using VoltStream.Domain.Enums;

public record UserDto
{
    public long Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}
