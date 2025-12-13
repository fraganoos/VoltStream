namespace VoltStream.Application.Features.Users.Commands;

using BCrypt.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Extensions;
using VoltStream.Application.Commons.Interfaces;

public record LoginUserCommand(
    string Username,
    string Password) : IRequest<bool>;

public class LoginUserCommandHandler(IAppDbContext context)
    : IRequestHandler<LoginUserCommand, bool>
{
    public async Task<bool> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.NormalizedUsername == request.Username.ToNormalized(), cancellationToken)
            ?? throw new NotFoundException("Login yoki parol xato!");

        bool isPasswordValid = BCrypt.Verify(
            request.Password,
            user.PasswordHash
        );

        if (!isPasswordValid)
            throw new NotFoundException("Login yoki parol xato!");

        return true;
    }
}