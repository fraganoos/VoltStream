namespace VoltStream.Application.Features.Users.Commands;

using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using VoltStream.Application.Commons.Exceptions;
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
            .FirstOrDefaultAsync(u => u.Username == request.Username, cancellationToken)
            ?? throw new NotFoundException("Login yoki parol xato!");

        using var hmac = new HMACSHA512(user.PasswordSalt);
        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password));

        if (!computedHash.SequenceEqual(user.PasswordHash))
            throw new UnauthorizedAccessException("Login yoki parol xato!");

        return true;
    }
}