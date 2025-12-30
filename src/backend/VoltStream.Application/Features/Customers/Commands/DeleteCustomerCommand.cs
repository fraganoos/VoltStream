namespace VoltStream.Application.Features.Customers.Commands;

using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;

public record DeleteCustomerCommand(long Id) : IRequest<bool>;

public class DeleteCustomerCommandHandler(
    IAppDbContext context)
    : IRequestHandler<DeleteCustomerCommand, bool>
{
    public async Task<bool> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
    {
        var customer = await context.Customers
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Customer), nameof(request.Id), request.Id);

        context.Customers.Remove(customer);

        try
        {
            await context.SaveAsync(cancellationToken);
        }
        catch (DbUpdateException)
        {
            throw new ForbiddenException("Ushbu mijozda operatsiyalar mavjud bo'lgani uchun o'chirib bo'lmaydi.");
        }

        return true;
    }
}
