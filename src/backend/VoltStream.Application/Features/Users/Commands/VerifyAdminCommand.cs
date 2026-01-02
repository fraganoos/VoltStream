namespace VoltStream.Application.Features.Users.Commands;

using BCrypt.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Extensions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Enums;

public record VerifyAdminCommand(string Username, string Password) : IRequest<bool>;

public class VerifyAdminCommandHandler(IAppDbContext context)
    : IRequestHandler<VerifyAdminCommand, bool>
{
    public async Task<bool> Handle(VerifyAdminCommand request, CancellationToken cancellationToken)
    {
        var user = await context.Users
            .FirstOrDefaultAsync(u => u.NormalizedUsername == request.Username.ToNormalized(), cancellationToken);

        if (user == null || user.Role != UserRole.Admin || !user.IsActive)
            return false;

        return BCrypt.Verify(request.Password, user.PasswordHash);
    }
}
