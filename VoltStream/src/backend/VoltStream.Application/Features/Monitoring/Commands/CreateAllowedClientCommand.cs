namespace VoltStream.Application.Features.Monitoring.Commands;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;

public record CreateAllowedClientCommand(
    string IpAddress,
    string? DeviceName,
    bool IsBlocked)
    : IRequest<long>;

public class CreateAllowedClientCommandHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<CreateAllowedClientCommand, long>
{
    public async Task<long> Handle(CreateAllowedClientCommand request, CancellationToken cancellationToken)
    {
        _ = await context.AllowedClients.FirstOrDefaultAsync(wh => wh.NormalizedName == request.IpAddress, cancellationToken)
            ?? throw new AlreadyExistException(nameof(AllowedClient), nameof(request.IpAddress), request.IpAddress);

        var client = mapper.Map<AllowedClient>(request);
        context.AllowedClients.Add(client);
        await context.SaveAsync(cancellationToken);
        return client.Id;
    }
}
