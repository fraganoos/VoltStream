namespace VoltStream.Application.Features.DiscountOperations.Commands;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;
using VoltStream.Domain.Enums;

public record ApplyDiscountCommand(
    long CustomerId,
    decimal DiscountAmount,
    bool IsCash,
    string Description)
    : IRequest<long>;

public class ApplyDiscountCommandHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<ApplyDiscountCommand, long>
{
    public async Task<long> Handle(ApplyDiscountCommand request, CancellationToken cancellationToken)
    {
        await context.BeginTransactionAsync(cancellationToken);

        try
        {
            var customer = await context.Customers
                .Include(c => c.Accounts)
                    .ThenInclude(a => a.Currency)
                .FirstOrDefaultAsync(c => c.Id == request.CustomerId, cancellationToken)
                ?? throw new NotFoundException(nameof(Customer), nameof(request.CustomerId), request.CustomerId);

            var account = customer.Accounts.FirstOrDefault()
                ?? throw new NotFoundException("Mijoz hisobi topilmadi");

            if (request.DiscountAmount <= 0)
                throw new AppException("Chegirma summasi 0 dan katta bo'lishi kerak");

            if (account.Discount < request.DiscountAmount)
                throw new ForbiddenException($"Yetarli chegirma yo'q. Mavjud: {account.Discount}, So'ralgan: {request.DiscountAmount}");

            if (request.IsCash)
            {
                var cash = await context.Cashes
                    .FirstOrDefaultAsync(c => c.CurrencyId == account.CurrencyId && c.IsActive, cancellationToken)
                    ?? throw new NotFoundException("Faol kassa topilmadi");

                if (cash.Balance < request.DiscountAmount)
                    throw new ForbiddenException($"Kassada yetarli mablag' yo'q. Mavjud: {cash.Balance}, Kerak: {request.DiscountAmount}");

                cash.Balance -= request.DiscountAmount;
            }
            else
            {
                account.Balance += request.DiscountAmount;
            }

            account.Discount -= request.DiscountAmount;

            var discountOperation = new DiscountOperation
            {
                Date = DateTimeOffset.UtcNow.UtcDateTime,
                Description = GenerateDescription(request, account.Currency),
                IsApplied = true,
                Amount = request.DiscountAmount,
                CustomerId = customer.Id,
                AccountId = account.Id,
                CreatedAt = DateTime.UtcNow
            };

            context.DiscountOperations.Add(discountOperation);

            var customerOperation = new CustomerOperation
            {
                Date = DateTime.UtcNow,
                AccountId = account.Id,
                Amount = request.DiscountAmount,
                CustomerId = customer.Id,
                Description = discountOperation.Description,
                CreatedAt = DateTime.UtcNow,
                OperationType = OperationType.DiscountApplied
            };

            context.CustomerOperations.Add(customerOperation);

            await context.CommitTransactionAsync(cancellationToken);
            return discountOperation.Id;
        }
        catch
        {
            await context.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    private static string GenerateDescription(ApplyDiscountCommand request, Currency currency)
    {
        var typeText = request.IsCash ? "Naqd chegirma" : "Naqd bo'lmagan chegirma";
        var additionalInfo = string.IsNullOrEmpty(request.Description)
            ? ""
            : $". {request.Description}";

        return $"{typeText}: {request.DiscountAmount} {currency.Code}{additionalInfo}";
    }
}