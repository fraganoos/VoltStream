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
                .ThenInclude(co => co.Cash)
            .Include(p => p.Account)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Payment), nameof(request.Id), request.Id);

        var cash = payment.CashOperation.Cash;
        if (payment.CashOperation.CurrencyType == CurrencyType.USD &&
            payment.CashOperation.Summa <= cash.UsdBalance)
            cash.UsdBalance -= payment.CashOperation.Summa + request.Summa;
        else if (payment.CashOperation.CurrencyType == CurrencyType.UZS &&
            payment.CashOperation.Summa <= cash.UzsBalance)
        {
            cash.UzsBalance -= payment.CashOperation.Summa + request.Summa;
        }
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
    {
        return request.PaymentType switch
        {
            PaymentType.Cash => request.CurrencyType switch
            {
                CurrencyType.UZS => $"Naqd pul orqali. Summa: {request.Summa} UZS.",
                CurrencyType.USD => $"Naqd pul orqali. Summa: {request.Summa} USD. Kurs: {request.Kurs} UZS'dan jami: {request.DefaultSumm}.",
                _ => request.Description
            },
            PaymentType.BankAccount => request.CurrencyType switch
            {
                CurrencyType.UZS => $"Hisob raqami orqali. Summa: {request.Summa} UZS. {request.Kurs}% dan jami: {request.DefaultSumm} UZS.",
                _ => request.Description
            },
            PaymentType.Mobile => request.CurrencyType switch
            {
                CurrencyType.UZS => $"Online orqali. Summa: {request.Summa} UZS. {request.Kurs}% dan jami: {request.DefaultSumm} UZS.",
                _ => request.Description
            },
            PaymentType.Card => request.CurrencyType switch
            {
                CurrencyType.UZS => $"Plastik karta orqali. Summa: {request.Summa} UZS. {request.Kurs}% dan jami: {request.DefaultSumm} UZS.",
                _ => request.Description
            },
            _ => request.Description
        };
    }
}
