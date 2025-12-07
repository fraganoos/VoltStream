namespace VoltStream.Application.Features.Payments.Commands;

using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;

public record UpdatePaymentCommand(
    long Id,
    DateTimeOffset PaidAt,
    long CustomerId,
    decimal Amount,
    long CurrencyId,
    decimal ExchangeRate,
    decimal NetAmount,
    string Description)
    : IRequest<bool>;

public class UpdatePaymentCommandHandler(IAppDbContext context)
    : IRequestHandler<UpdatePaymentCommand, bool>
{
    public async Task<bool> Handle(UpdatePaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await context.Payments
            .Include(p => p.CustomerOperation)
                .ThenInclude(co => co.Account)
            .Include(p => p.Customer)
            .Include(p => p.Currency)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Payment), nameof(request.Id), request.Id);

        var currency = payment.Currency;

        var cash = currency.IsCash
            ? await EnsureCashExists(currency.Id, cancellationToken)
            : null;

        await context.BeginTransactionAsync(cancellationToken);

        try
        {
            RevertOldValues(payment, cash);
            ValidateCashBalance(payment, request, cash);
            ApplyNewValues(payment, request, cash, currency);

            return await context.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await context.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private async Task<Cash> EnsureCashExists(long currencyId, CancellationToken cancellationToken)
    {
        var cash = await context.Cashes
            .FirstOrDefaultAsync(c => c.CurrencyId == currencyId, cancellationToken);

        if (cash is not null) return cash;

        var newCash = new Cash
        {
            CurrencyId = currencyId,
            Balance = 0
        };

        context.Cashes.Add(newCash);
        return newCash;
    }

    private static void RevertOldValues(Payment payment, Cash? cash)
    {
        var oldAmount = payment.Amount;
        var oldNetAmount = payment.NetAmount;

        if (payment.Currency.IsCash && cash is not null)
            cash.Balance -= oldAmount;

        payment.CustomerOperation.Account.Balance -= oldNetAmount;
    }

    private static void ValidateCashBalance(Payment payment, UpdatePaymentCommand request, Cash? cash)
    {
        var diffAmount = request.Amount - payment.Amount;

        if (payment.Currency.IsCash && diffAmount < 0 && cash!.Balance < Math.Abs(diffAmount))
            throw new ConflictException("Kassada mablag' yetarli emas!");
    }

    private static void ApplyNewValues(Payment payment, UpdatePaymentCommand request, Cash? cash, Currency currency)
    {
        var diffAmount = request.Amount - payment.Amount;
        var diffNet = request.NetAmount - payment.NetAmount;

        if (currency.IsCash && cash is not null)
            cash.Balance += diffAmount;

        payment.PaidAt = request.PaidAt;
        payment.Amount = request.Amount;
        payment.ExchangeRate = request.ExchangeRate;
        payment.NetAmount = request.NetAmount;
        payment.Description = request.Description;
        payment.CurrencyId = request.CurrencyId;

        payment.CustomerOperation.Account.Balance += diffNet;

        if (payment.CustomerOperation is not null)
        {
            payment.CustomerOperation.Amount = request.Amount;
            payment.CustomerOperation.Description = GenerateDescription(request, currency);
        }
    }

    private static string GenerateDescription(UpdatePaymentCommand request, Currency currency)
    {
        var typeText = currency.IsCash ? "Naqd to‘lov" : "Naqd bo‘lmagan to‘lov";
        return $"{typeText}: {request.NetAmount} {currency.Code}. Kurs: {request.ExchangeRate}";
    }
}
