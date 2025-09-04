namespace VoltStream.Application.Features.Payments.Commands;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;
using VoltStream.Domain.Enums;

public record UpdatePaymentCommand(
    long Id,
    DateTimeOffset PaidDate,
    long CustomerId,
    PaymentType PaymentType,
    decimal Summa,
    CurrencyType CurrencyType,
    decimal Kurs,
    decimal DefaultSumm,
    string Description,
    long CustomerOperation) : IRequest<long>;

public class UpdatePaymentCommandHandler(
    IAppDbContext context,
    IMapper mapper) : IRequestHandler<UpdatePaymentCommand, long>
{
    public async Task<long> Handle(UpdatePaymentCommand request, CancellationToken cancellationToken)
    {
        var payment = await context.Payments
            .Include(payment => payment.CustomerOperation)
            .Include(p => p.CashOperation)
            .Include(p => p.Account)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Payment), nameof(request.Id), request.Id);

        var cash = await context.Cashes.FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(nameof(Cash));

        var difference = payment.CashOperation.Summa - request.Summa;
        var isAvailable = difference <= cash.UsdBalance;

        if (payment.CashOperation.CurrencyType == CurrencyType.USD && isAvailable)
            cash.UsdBalance -= difference;
        else if (payment.CashOperation.CurrencyType == CurrencyType.UZS && isAvailable)
            cash.UzsBalance -= difference;
        else
            throw new ConflictException("Kassada mablag' yetarli emas!");

        await context.BeginTransactionAsync(cancellationToken);

        mapper.Map(request, payment);
        mapper.Map(request, payment.CustomerOperation);
        payment.CustomerOperation.Description = GenerateDescription(request);

        await context.CommitTransactionAsync(cancellationToken);
        return payment.Id;
    }

    private static string GenerateDescription(UpdatePaymentCommand request)
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
