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
    CurrencyType CurrencyType,
    decimal Kurs,
    decimal DefaultSumm,
    string Description) : IRequest<long>;

public class CreatePaymentCommandHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<CreatePaymentCommand, long>
{
    public async Task<long> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
    {
        var customer = context.Customers
            .Include(c => c.Account)
            .FirstOrDefault(dk => dk.Id == request.CustomerId)
            ?? throw new NotFoundException(nameof(Account));
        var account = customer.Account;

        if (request.PaymentType == PaymentType.Cash)
        {
            var cash = await context.Cashes.FirstOrDefaultAsync(cancellationToken)
                ?? throw new NotFoundException(nameof(Cash));

            context.CashOperations.Add(new CashOperation
            {
                CurrencyType = request.CurrencyType,
                Date = DateTime.UtcNow,
                Description = $"ID = {request.CustomerId}. Mijozdan kirim",
                Summa = request.Summa
            });

            if (request.CurrencyType == CurrencyType.UZS)
                cash.UzsBalance += request.Summa;
            else
                cash.UsdBalance += request.Summa;
        }

        await context.BeginTransactionAsync(cancellationToken);

        account.CurrentSumm += request.DefaultSumm;

        var payment = mapper.Map<Payment>(request);
        var customerOperation = mapper.Map<CustomerOperation>(request);
        customerOperation.OperationType = OperationType.Payment;
        customerOperation.Description = GenerateDescription(request);
        payment.CustomerOperation = customerOperation;

        context.Payments.Add(payment);

        await context.CommitTransactionAsync(cancellationToken);
        return payment.Id;
    }

    private static string GenerateDescription(CreatePaymentCommand request)
        => request.PaymentType switch
        {
            PaymentType.Cash => request.CurrencyType switch
            {
                CurrencyType.UZS => $"Naqd: {request.Summa} UZS.",
                CurrencyType.USD => $"Naqd: {request.Summa} USD. Kurs: {request.Kurs} UZS",
                _ => request.Description
            },
            PaymentType.BankAccount => request.CurrencyType switch
            {
                CurrencyType.UZS => $"Bank: {request.Summa} UZS. {request.Kurs}% dan",
                _ => request.Description
            },
            PaymentType.Mobile => request.CurrencyType switch
            {
                CurrencyType.UZS => $"Online: {request.Summa} UZS. {request.Kurs}% dan.",
                _ => request.Description
            },
            PaymentType.Card => request.CurrencyType switch
            {
                CurrencyType.UZS => $"Plastik: {request.Summa} UZS. {request.Kurs}% dan",
                _ => request.Description
            },
            _ => request.Description
        };
}
