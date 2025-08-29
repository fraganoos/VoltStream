namespace VoltStream.Application.Features.Payments.Commands;

using AutoMapper;
using MediatR;
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
        await context.BeginTransactionAsync(cancellationToken);

        var customerOperation = mapper.Map<CustomerOperation>(request);
        customerOperation.OperationType = OperationType.Payment;
        customerOperation.Description = GenerateDescription(request);

        var payment = mapper.Map<Payment>(request);
        payment.CustomerOperation = customerOperation;

        context.Payments.Add(payment);
        context.CustomerOperations.Add(customerOperation);

        await context.CommitTransactionAsync(cancellationToken);
        return payment.Id;
    }

    private static string GenerateDescription(CreatePaymentCommand request)
        => request.PaymentType switch
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
