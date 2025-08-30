namespace VoltStream.Application.Features.DebtKredits.Commands;

using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;

public record DeleteDebtKreditCommand(long Id) : IRequest<bool>;

public class DeleteDebtKreditCommandHandler(
    IAppDbContext context)
    : IRequestHandler<DeleteDebtKreditCommand, bool>
{
    public async Task<bool> Handle(DeleteDebtKreditCommand request, CancellationToken cancellationToken)
    {
        var debtKredit = await context.DebtKredits.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Product), nameof(request.Id), request.Id);

        debtKredit.IsDeleted = true;
        context.DebtKredits.Update(debtKredit);
        return await context.SaveAsync(cancellationToken) > 0;
    }
}
