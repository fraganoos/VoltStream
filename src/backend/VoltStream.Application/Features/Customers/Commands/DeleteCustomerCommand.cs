namespace VoltStream.Application.Features.Customers.Commands;

using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
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
        // Mijozni olish va accountlarini yuklash
        var customer = await context.Customers
            .Include(c => c.Accounts)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Customer), nameof(request.Id), request.Id);

        foreach (var account in customer.Accounts)
        {
            if (account.Balance > 0)
                throw new ForbiddenException($"Mijoz haqdor: {account.Balance}");
            if (account.Balance < 0)
                throw new ForbiddenException($"Mijoz qarzdor: {account.Balance}");
            if (account.Discount > 0)
                throw new ForbiddenException($"Mijozda chegirma mavjud: {account.Discount}");
        }

        await context.BeginTransactionAsync(cancellationToken);

        try
        {
            // Soft delete bajarish
            customer.IsDeleted = true;
            foreach (var account in customer.Accounts)
                account.IsDeleted = true;

            var result = await context.CommitTransactionAsync(cancellationToken);
            return result;
        }
        catch
        {
            await context.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
