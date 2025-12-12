namespace VoltStream.Application.Features.Users.Commands;

using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Extensions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;

public record CreateUserCommand(
    string Username,
    string Password) : IRequest<long>;

public class CreateUserCommandHandler(
    IAppDbContext context)
    : IRequestHandler<CreateUserCommand, long>
{
    public async Task<long> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var exitUser = await context.Users.AnyAsync(u => u.Username == request.Username, cancellationToken);

        if (exitUser)
            throw new AlreadyExistException(nameof(User), nameof(request.Username), request.Username);

        using var hmac = new HMACSHA512();
        var salt = hmac.Key;
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(request.Password));

        var user = new User
        {
            Username = request.Username,
            NormalizedUsername = request.Username.ToNormalized(),
            PasswordHash = hash,
            PasswordSalt = salt
        };

        context.Users.Add(user);
        await context.SaveAsync(cancellationToken);

        return user.Id;
    }
}
