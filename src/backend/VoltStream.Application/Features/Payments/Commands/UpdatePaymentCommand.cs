namespace VoltStream.Application.Features.Payments.Commands;

using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Features.CustomerOperations.Commands;
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

public class UpdatePaymentCommandHandler(
    IAppDbContext context,
    IMediator mediator)
    : IRequestHandler<UpdatePaymentCommand, bool>
{
    public async Task<bool> Handle(UpdatePaymentCommand request, CancellationToken cancellationToken)
    {
        var oldPayment = await context.Payments
            .Include(p => p.CustomerOperation)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Payment), nameof(request.Id), request.Id);

        if (oldPayment.CustomerOperation is null)
            throw new ConflictException("Tahrirlash uchun Payment yozuviga bog'langan CustomerOperation topilmadi.");

        try
        {
            var deleteCommand = new DeleteCustomerOperationCommand(oldPayment.CustomerOperation.Id);
            await mediator.Send(deleteCommand, cancellationToken);
        }
        catch (Exception ex)
        {
            throw new ConflictException($"Eski to'lov amalini qaytarishda xatolik yuz berdi: {ex.Message}");
        }

        var createCommand = new CreatePaymentCommand(
            request.PaidAt,
            request.CustomerId,
            request.Amount,
            request.CurrencyId,
            request.ExchangeRate,
            request.NetAmount,
            request.Description
        );

        await mediator.Send(createCommand, cancellationToken);
        return true;
    }
}