namespace VoltStream.Application.Features.Users.DTOs;

using VoltStream.Domain.Enums;

public record UserDto(
    long Id,
    string Username,
    string Password,
    UserRole Role);
