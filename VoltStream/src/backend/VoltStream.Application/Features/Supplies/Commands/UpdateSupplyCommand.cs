namespace VoltStream.Application.Features.Supplies.Commands;

using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;

public record UpdateSupplyCommand(
    long Id,
    DateTime Date,
    decimal RollCount,
    decimal LengthPerRoll,
    decimal TotalLength) : IRequest<bool>;

public class UpdateSupplyCommandHandler(
    IAppDbContext context)
    : IRequestHandler<UpdateSupplyCommand, bool>
{
    public async Task<bool> Handle(UpdateSupplyCommand request, CancellationToken cancellationToken)
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

        await context.BeginTransactionAsync(cancellationToken);

        try
        {
            // Eski supply miqdorini warehouse stockga qaytarish
            stock.RollCount -= supply.RollCount;
            stock.TotalLength -= supply.TotalLength;

            // Supply ma'lumotlarini yangilash
            supply.Date = request.Date;
            supply.RollCount = request.RollCount;
            supply.LengthPerRoll = request.LengthPerRoll;
            supply.TotalLength = request.TotalLength;

            // WarehouseStockni yangilash yangi supply bilan
            stock.RollCount += request.RollCount;
            stock.TotalLength += request.TotalLength;

            return await context.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await context.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}
