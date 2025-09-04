namespace VoltStream.Application.Features.Supplies.Commands;

using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;

public record DeleteSupplyCommand(long Id) : IRequest<bool>;

public class DeleteSupplyCommandHandler(
    IAppDbContext context)
    : IRequestHandler<DeleteSupplyCommand, bool>
{
    public async Task<bool> Handle(DeleteSupplyCommand request, CancellationToken cancellationToken)
    {
        var supply = await context.Supplies.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Supply), nameof(request.Id), request.Id);

        var resedue = await context.WarehouseItems
            .FirstOrDefaultAsync(wh => wh.ProductId == supply.ProductId &&
                wh.QuantityPerRoll == supply.QuantityPerRoll, cancellationToken)
            ?? throw new NotFoundException(nameof(WarehouseItem), nameof(request.Id), request.Id);

        if (resedue.TotalQuantity < supply.TotalQuantity)
            throw new ConflictException($"Omborda bu maxsulotdan faqat {resedue.TotalQuantity} metr mavjud!");

        await context.BeginTransactionAsync(cancellationToken);

        resedue.TotalQuantity -= supply.TotalQuantity;
        resedue.CountRoll -= supply.CountRoll;
        supply.IsDeleted = true;

        return await context.CommitTransactionAsync(cancellationToken);
    }
}