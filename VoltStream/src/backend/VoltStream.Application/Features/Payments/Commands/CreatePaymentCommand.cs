namespace VoltStream.Application.Features.Payments.Commands;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;
using VoltStream.Domain.Enums;

public record CreatePaymentCommand(
    DateTimeOffset PaidDate,
    long CustomerId,
    PaymentType PaymentType,
    decimal Summa,
    long CurrencyId,
    decimal ExchangeRate,
    decimal NetAmount,
    string Description) : IRequest<long>;

public class CreatePaymentCommandHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<CreatePaymentCommand, long>
{
    public async Task<long> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        await context.BeginTransactionAsync(cancellationToken);

        try
        {
            // === 1. Asosiy entitylarni olish ===
            var customer = await context.Customers
                .Include(c => c.Accounts)
                .FirstOrDefaultAsync(c => c.Id == request.CustomerId, cancellationToken)
                ?? throw new NotFoundException(nameof(Customer), nameof(request.CustomerId), request.CustomerId);

            var currency = await context.Currencies
                .FirstOrDefaultAsync(c => c.Id == request.CurrencyId, cancellationToken)
                ?? throw new NotFoundException(nameof(Currency), nameof(request.CurrencyId), request.CurrencyId);

            // === 2. Mijoz uchun valyuta hisobini aniqlash ===
            var account = customer.Accounts
                .FirstOrDefault(a => a.CurrencyId == request.CurrencyId)
                ?? throw new ConflictException("Ushbu valyutada mijoz uchun hisob mavjud emas.");

            // === 3. PaymentType bo‘yicha kassa yoki boshqa balansni yangilash ===
            Cash? cash = null;
            if (request.PaymentType == PaymentType.Cash)
            {
                cash = await context.Cashes
                    .FirstOrDefaultAsync(c => c.CurrencyId == request.CurrencyId, cancellationToken)
                    ?? throw new NotFoundException(nameof(Cash), nameof(request.CurrencyId), request.CurrencyId);

                cash.Balance += request.Summa;

                context.CashOperations.Add(new CashOperation
                {
                    CurrencyId = request.CurrencyId,
                    Date = DateTime.UtcNow,
                    Description = GenerateDescription(request),
                    Amount = request.Summa
                });
            }

            // === 4. Mijoz account balansini yangilash ===
            account.Balance += request.NetAmount;
            currency.ExchangeRate = request.ExchangeRate;

            // === 5. Payment va CustomerOperation yaratish ===
            var payment = mapper.Map<Payment>(request);
            payment.CurrencyId = request.CurrencyId;
            payment.AccountId = account.Id;

            var customerOperation = new CustomerOperation
            {
                AccountId = account.Id,
                OperationType = OperationType.Payment,
                Amount = request.NetAmount,
                Description = GenerateDescription(request),
                CreatedAt = DateTime.UtcNow
            };

            payment.CustomerOperation = customerOperation;

            context.Payments.Add(payment);

            // === 6. Transactionni commit qilish ===
            await context.CommitTransactionAsync(cancellationToken);

            return payment.Id;
        }
        catch
        {
            await context.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    // === 7. Description generatsiyasi ===
    private static string GenerateDescription(CreatePaymentCommand request)
        => request.PaymentType switch
        {
            PaymentType.Cash => $"Naqd to‘lov: {request.Summa} {GetCurrencyCode(request.CurrencyId)}. Kurs: {request.ExchangeRate}",
            PaymentType.BankAccount => $"Bank orqali to‘lov: {request.Summa} {GetCurrencyCode(request.CurrencyId)}. Kurs: {request.ExchangeRate}",
            PaymentType.Card => $"Karta orqali to‘lov: {request.Summa} {GetCurrencyCode(request.CurrencyId)}. Kurs: {request.ExchangeRate}",
            PaymentType.Mobile => $"Mobil to‘lov: {request.Summa} {GetCurrencyCode(request.CurrencyId)}. Kurs: {request.ExchangeRate}",
            _ => request.Description
        };

    private static string GetCurrencyCode(long currencyId)
        => currencyId switch
        {
            1 => "UZS",
            2 => "USD",
            _ => "VALYUTA"
        };
}
