namespace VoltStream.Application.Features.Cashes.Commands;

using MediatR;
using VoltStream.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;

public record DeleteCashCommand(long Id) : IRequest<bool>;


public class DeleteCashCommandHandler(
    IAppDbContext context)
    : IRequestHandler<DeleteCashCommand, bool>
{
    public async Task<bool> Handle(DeleteCashCommand request, CancellationToken cancellationToken)
    {
        var cash = await context.Cashes.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Cash), nameof(request.Id), request.Id);

        cash.IsDeleted = true;
        context.Cashes.Update(cash);
        return await context.SaveAsync(cancellationToken) > 0;
    }
}
