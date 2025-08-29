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
        var payment = await context.Payments.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Payment), nameof(request.Id), request.Id);

        var customerOperation = await context.CustomerOperations.FirstOrDefaultAsync(co => co.CustomerId == payment.CustomerId, cancellationToken)
            ?? throw new NotFoundException(nameof(CustomerOperation), nameof(payment.CustomerId), payment.CustomerId);

        await using var transaction = await context.BeginTransactionAsync(cancellationToken);
        try
        {
            payment.IsDeleted = true;
            customerOperation.IsDeleted = true;

            var result = await context.SaveAsync(cancellationToken) > 0;
            await transaction.CommitAsync(cancellationToken);
            return result;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }
}