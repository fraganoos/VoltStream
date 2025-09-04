namespace VoltStream.Application.Features.Customers.Commands;

using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;

public record DeleteCostumerCommand(long Id) : IRequest<bool>;

public class DeleteCostumerCommandHandler(
    IAppDbContext context)
    : IRequestHandler<DeleteCostumerCommand, bool>
{
    public async Task<bool> Handle(DeleteCostumerCommand request, CancellationToken cancellationToken)
    {
        var customer = await context.Customers
            .Include(c => c.Account)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Customer), nameof(request.Id), request.Id);

        if (customer.Account.CurrentSumm > 0)
            throw new ForbiddenException($"Mijoz haqdor: {customer.Account.CurrentSumm}");
        else if (customer.Account.CurrentSumm < 0)
            throw new ForbiddenException($"Mijoz qarzdor: {customer.Account.CurrentSumm}");

        if (customer.Account.DiscountSumm > 0)
            throw new ForbiddenException($"Mijozda chegirma mavjud: {customer.Account.DiscountSumm}");

        customer.IsDeleted = true;
        customer.Account.IsDeleted = true;
        return await context.SaveAsync(cancellationToken) > 0;
    }
}