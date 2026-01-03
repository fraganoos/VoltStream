namespace VoltStream.Application.Features.Users.DTOs;

using VoltStream.Domain.Enums;

public record AuthResponse(
    long Id,
    string Username,
    string Token,
    UserRole Role);
