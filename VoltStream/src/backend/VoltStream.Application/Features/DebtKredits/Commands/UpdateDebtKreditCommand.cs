namespace VoltStream.Application.Features.DebtKredits.Commands;

using MediatR;
using AutoMapper;
using VoltStream.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Application.Commons.Exceptions;

public record UpdateDebtKreditCommand(
    long Id,
    long CustomerId,
    decimal BeginSumm,
    decimal CurrencySumm,
    bool IsActive) : IRequest<long>;

public class UpdateDebtKreditCommandHandler(
    IAppDbContext context,IMapper mapper)
    : IRequestHandler<UpdateDebtKreditCommand, long>
{
    public async Task<long> Handle(UpdateDebtKreditCommand request, CancellationToken cancellationToken)
    {
        var debtKredit = await context.DebtKredits.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(DebtKredit), nameof(request.Id), request.Id);

        mapper.Map(request, debtKredit);
        debtKredit.UpdatedAt = DateTime.UtcNow;
        await context.SaveAsync(cancellationToken);
        return debtKredit.Id;
    }
}