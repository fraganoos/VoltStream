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
        await context.BeginTransactionAsync(cancellationToken);

        try
        {
            // === 1. Paymentni barcha bog‘lamalari bilan olish ===
            var payment = await context.Payments
                .Include(p => p.CustomerOperation)
                    .ThenInclude(co => co.Account)
                .Include(p => p.Customer)
                .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Payment), nameof(request.Id), request.Id);



            // === 2. Agar kassa orqali to‘lov bo‘lgan bo‘lsa, kassadagi balansni kamaytirish ===
            if (payment.Type == Domain.Enums.PaymentType.Cash)
            {
                var cash = await context.Cashes
                    .FirstOrDefaultAsync(c => c.CurrencyId == payment.CurrencyId, cancellationToken)
                    ?? throw new NotFoundException(nameof(Cash), nameof(payment.CurrencyId), payment.CurrencyId);

                if (cash.Balance < payment.Amount)
                    throw new ConflictException("Kassada mablag‘ yetarli emas!");

                cash.Balance -= payment.Amount;
            }

            // === 3. Mijoz hisobidagi balansni kamaytirish ===
            payment.CustomerOperation.Account.Balance -= payment.NetAmount;

            // === 4. Payment va bog‘liq operatsiyalarni soft delete qilish ===
            payment.IsDeleted = true;

            if (payment.CustomerOperation is not null)
                payment.CustomerOperation.IsDeleted = true;

            if (payment.DiscountOperation is not null)
                payment.DiscountOperation.IsDeleted = true;

            // === 5. Saqlash va commit ===
            await context.CommitTransactionAsync(cancellationToken);

            return true;
        }
        catch
        {
            await context.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
