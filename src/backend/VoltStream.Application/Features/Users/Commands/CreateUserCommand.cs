namespace VoltStream.Application.Features.Users.Commands;

using AutoMapper;
using BCrypt.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Extensions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;
using VoltStream.Domain.Enums;

public record CreateUserCommand(
    string Username,
    string Password,
    string? Name,
    UserRole Role,
    string? Phone,
    string? Email,
    string? Address,
    DateTime? DateOfBirth)
    : IRequest<long>;

public class CreateUserCommandHandler(IAppDbContext context, IMapper mapper)
    : IRequestHandler<CreateUserCommand, long>
{
    public async Task<long> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var normalizedUsername = request.Username.ToNormalized();
        var exitUser = await context.Users.AnyAsync(
            u => u.NormalizedUsername == normalizedUsername,
            cancellationToken);

        if (exitUser)
            throw new AlreadyExistException(nameof(User), nameof(request.Username), request.Username);

        if (!string.IsNullOrEmpty(request.Email))
        {
            var normalizedEmail = request.Email.ToNormalized();
            var exitEmail = await context.Users.AnyAsync(
                u => u.NormalizedEmail == normalizedEmail,
                cancellationToken);

            if (exitEmail)
                throw new AlreadyExistException(nameof(User), nameof(request.Email), request.Email);
        }

        var user = mapper.Map<User>(request);
        user.PasswordHash = BCrypt.HashPassword(request.Password, workFactor: 12);

        context.Users.Add(user);
        await context.SaveAsync(cancellationToken);

        return user.Id;
    }
}