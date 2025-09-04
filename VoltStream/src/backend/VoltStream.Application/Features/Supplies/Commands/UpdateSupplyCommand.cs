namespace VoltStream.Application.Features.Supplies.Commands;

using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;

public record UpdateSupplyCommand(
    long Id,
    DateTimeOffset OperationDate,
    long ProductId,
    decimal CountRoll,
    decimal QuantityPerRoll,
    decimal TotalQuantity) : IRequest<long>;

public class UpdateSupplyCommandHandler(
    IAppDbContext context)
    : IRequestHandler<UpdateSupplyCommand, long>
{
    public async Task<long> Handle(UpdateSupplyCommand request, CancellationToken cancellationToken)
    {
        var supply = await context.Supplies.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Supply), nameof(request.Id), request.Id);

        var resedue = await context.WarehouseItems
            .FirstOrDefaultAsync(wh => wh.ProductId == supply.ProductId &&
                wh.QuantityPerRoll == supply.QuantityPerRoll, cancellationToken)
            ?? throw new NotFoundException(nameof(WarehouseItem), nameof(request.Id), request.Id);

        await context.BeginTransactionAsync(cancellationToken);

        resedue.TotalQuantity -= supply.TotalQuantity + request.TotalQuantity;
        resedue.CountRoll -= supply.CountRoll + request.CountRoll;

        await context.CommitTransactionAsync(cancellationToken);
        return supply.Id;
    }
}