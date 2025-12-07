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
        // Supply ni olish
        var supply = await context.Supplies
            .FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Supply), nameof(request.Id), request.Id);

        // WarehouseStock ni olish
        var stock = await context.WarehouseStocks
            .FirstOrDefaultAsync(ws => ws.ProductId == supply.ProductId &&
                                       ws.LengthPerRoll == supply.LengthPerRoll,
                                   cancellationToken)
            ?? throw new NotFoundException(nameof(WarehouseStock), nameof(supply.Id), supply.Id);

        if (stock.TotalLength < supply.TotalLength)
            throw new ConflictException($"Omborda bu mahsulotdan faqat {stock.TotalLength} metr mavjud!");

        await context.BeginTransactionAsync(cancellationToken);

        try
        {
            // WarehouseStockni kamaytirish
            stock.TotalLength -= supply.TotalLength;
            stock.RollCount -= supply.RollCount;

            // Supply ni soft delete qilish
            supply.IsDeleted = true;

            return await context.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await context.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
