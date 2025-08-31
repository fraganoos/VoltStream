namespace VoltStream.Application.Features.Supplies.Commands;

using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;

public record DeleteSupplyCommand(long Id) : IRequest<bool>;

public class DeleteSupplyCommandHandler(
    IAppDbContext context)
    : IRequestHandler<DeleteSupplyCommand, bool>
{
    public async Task<bool> Handle(DeleteSupplyCommand request, CancellationToken cancellationToken)
    {
        var supply = await context.Supplies.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Supply), nameof(request.Id), request.Id);
        supply.IsDeleted = true;
        context.Supplies.Update(supply);
        return await context.SaveAsync(cancellationToken) > 0;
    }
}