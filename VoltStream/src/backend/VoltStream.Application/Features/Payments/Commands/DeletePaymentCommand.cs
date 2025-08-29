namespace VoltStream.Application.Features.Payments.Commands;

using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;

public record DeletePaymentCommand(long Id) : IRequest<bool>;

public class DeletePaymentCommandHandler(
    IAppDbContext context)
    : IRequestHandler<DeletePaymentCommand, bool>
{
    public async Task<bool> Handle(DeletePaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await context.Payments
            .Include(p => p.CustomerOperation)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Payment), nameof(request.Id), request.Id);

        await context.BeginTransactionAsync(cancellationToken);

        payment.IsDeleted = true;
        payment.CustomerOperation.IsDeleted = true;

        return await context.CommitTransactionAsync(cancellationToken);
    }

}