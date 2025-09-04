namespace VoltStream.Application.Features.Payments.Commands;

using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;
using VoltStream.Domain.Enums;

public record DeletePaymentCommand(long Id) : IRequest<bool>;

public class DeletePaymentCommandHandler(
    IAppDbContext context)
    : IRequestHandler<DeletePaymentCommand, bool>
{
    public async Task<bool> Handle(DeletePaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await context.Payments
            .Include(p => p.CustomerOperation)
            .Include(p => p.CashOperation)
            .Include(p => p.Account)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Payment), nameof(request.Id), request.Id);

        var cash = await context.Cashes.FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(nameof(Cash));

        if (payment.CashOperation.CurrencyType == CurrencyType.USD &&
            payment.CashOperation.Summa <= cash.UsdBalance)
            cash.UsdBalance -= payment.CashOperation.Summa;
        else if (payment.CashOperation.CurrencyType == CurrencyType.UZS &&
            payment.CashOperation.Summa <= cash.UzsBalance)
            cash.UzsBalance -= payment.CashOperation.Summa;
        else
            throw new ConflictException("Kassada mablag' yetarli emas!");

        await context.BeginTransactionAsync(cancellationToken);

        payment.Account.CurrentSumm -= payment.Summa;
        payment.IsDeleted = true;
        payment.CustomerOperation.IsDeleted = true;
        payment.CashOperation.IsDeleted = true;

        return await context.CommitTransactionAsync(cancellationToken);
    }
}