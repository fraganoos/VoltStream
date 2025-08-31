namespace VoltStream.Application.Features.Sales.Commands;

using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;

public record DeleteSaleCommand(long Id) : IRequest<bool>;

public class DeleteSaleCommandHandler(
    IAppDbContext context)
    : IRequestHandler<DeleteSaleCommand, bool>
{
    public async Task<bool> Handle(DeleteSaleCommand request, CancellationToken cancellationToken)
    {
        var payment = await context.Sales
            .Include(p => p.CustomerOperation)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Sale), nameof(request.Id), request.Id);

        await context.BeginTransactionAsync(cancellationToken);

        payment.IsDeleted = true;
        payment.CustomerOperation.IsDeleted = true;

        return await context.CommitTransactionAsync(cancellationToken);
    }
}
