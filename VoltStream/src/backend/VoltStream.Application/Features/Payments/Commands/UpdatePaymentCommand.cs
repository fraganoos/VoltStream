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
        var payment = await context.Payments.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Payment), nameof(request.Id), request.Id);

        var customer = await context.CustomerOperations.FirstOrDefaultAsync(co => co.Id == request.CustomerOperation, cancellationToken)
            ?? throw new NotFoundException(nameof(CustomerOperation), nameof(request.Id), request.Id);

        await using var transaction = await context.BeginTransactionAsync(cancellationToken);

        try
        {
            mapper.Map(request, payment);

            mapper.Map(request, customer);
            customer.Description = GenerateDescription(request);

            await context.SaveAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return payment.Id;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
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
