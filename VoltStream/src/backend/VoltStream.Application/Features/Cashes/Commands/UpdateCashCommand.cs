namespace VoltStream.Application.Features.Cashes.Commands;

using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;

public record UpdateCashCommand(
    long Id,
    decimal UzsBalance,
    decimal UsdBalance,
    decimal Kurs) : IRequest<long>;

public class UpdateCashCommandHandler(
    IAppDbContext context)
    : IRequestHandler<UpdateCashCommand, long>
{
    public async Task<long> Handle(UpdateCashCommand request, CancellationToken cancellationToken)
    {
        var cash = await context.Cashes.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Cash), nameof(request.Id), request.Id);

        cash.UzsBalance = request.UzsBalance;
        cash.UsdBalance = request.UsdBalance;
        cash.Kurs = request.Kurs;
        await context.SaveAsync(cancellationToken);
        return cash.Id;
    }
}