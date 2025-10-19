namespace VoltStream.Application.Features.Payments.Commands;

using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;
using VoltStream.Domain.Enums;

public record UpdatePaymentCommand(
    long Id,
    DateTimeOffset PaidAt,
    long CustomerId,
    PaymentType Type,
    decimal Amount,
    long CurrencyId,
    decimal ExchangeRate,
    decimal NetAmount,
    string Description)
    : IRequest<bool>;

public class UpdatePaymentCommandHandler(
    IAppDbContext context)
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

        var cash = await context.Cashes
            .Include(c => c.Currency)
            .FirstOrDefaultAsync(c => c.CurrencyId == request.CurrencyId, cancellationToken)
            ?? throw new NotFoundException(nameof(Cash), nameof(request.CurrencyId), request.CurrencyId);

        await context.BeginTransactionAsync(cancellationToken);

        try
        {
            // 🔹 Eski qiymatlar
            var oldAmount = payment.Amount;
            var oldNetAmount = payment.NetAmount;

            // 🔹 Balans farqi
            var diffAmount = request.Amount - oldAmount;
            var diffNet = request.NetAmount - oldNetAmount;

            // 💰 Kassani tekshirish (mablag‘ yetarli bo‘lishi kerak)
            if (diffAmount < 0 && cash.Balance < Math.Abs(diffAmount))
                throw new ConflictException("Kassada mablag' yetarli emas!");

            // 🔹 Kassa balansini yangilash
            cash.Balance += diffAmount;

            // 🔹 Hisob balansini yangilash
            payment.CustomerOperation.Account.Balance += diffNet;

            // 🔹 Payment ma'lumotlarini yangilash
            payment.PaidAt = request.PaidAt;
            payment.Type = request.Type;
            payment.Amount = request.Amount;
            payment.ExchangeRate = request.ExchangeRate;
            payment.NetAmount = request.NetAmount;
            payment.Description = request.Description;
            payment.CurrencyId = request.CurrencyId;

            // 🔹 CustomerOperation yangilash
            if (payment.CustomerOperation is not null)
            {
                payment.CustomerOperation.Amount = request.Amount;
                payment.CustomerOperation.Description = GenerateDescription(request);
            }

            return await context.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await context.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private static string GenerateDescription(UpdatePaymentCommand request)
    {
        return request.Type switch
        {
            PaymentType.Cash => $"Naqd to‘lov: {request.Amount} {GetCurrencyName(request.CurrencyId)}. Kurs: {request.ExchangeRate}.",
            PaymentType.BankAccount => $"Bank orqali to‘lov: {request.Amount} {GetCurrencyName(request.CurrencyId)} ({request.ExchangeRate}% komissiya).",
            PaymentType.Mobile => $"Mobil to‘lov: {request.Amount} {GetCurrencyName(request.CurrencyId)} ({request.ExchangeRate}% foiz bilan).",
            PaymentType.Card => $"Karta orqali to‘lov: {request.Amount} {GetCurrencyName(request.CurrencyId)} ({request.ExchangeRate}% foiz).",
            _ => request.Description
        };
    }

    private static string GetCurrencyName(long currencyId)
    {
        // Bu joy keyinchalik Currency table’dan real nom olish uchun o‘zgartiriladi
        return currencyId switch
        {
            1 => "UZS",
            2 => "USD",
            _ => "Valyuta"
        };
    }
}
