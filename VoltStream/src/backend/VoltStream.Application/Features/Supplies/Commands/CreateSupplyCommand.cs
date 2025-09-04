namespace VoltStream.Application.Features.Supplies.Commands;

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;

public record CreateSupplyCommand(
    DateTimeOffset OperationDate,
    long ProductId,
    decimal CountRoll,
    decimal QuantityPerRoll,
    decimal TotalQuantity,
    decimal Price,
    decimal DiscountPercent)
    : IRequest<long>;

public class CreateSupplyCommandHandler(
    IAppDbContext context,
    IMapper mapper)
    : IRequestHandler<CreateSupplyCommand, long>
{
    public async Task<long> Handle(CreateSupplyCommand request, CancellationToken cancellationToken)
    {
        var warehouse = await context.Warehouses
            .Include(w => w.Items)
            .FirstOrDefaultAsync(cancellationToken);

        if (warehouse is null)
            context.Warehouses.Add(warehouse = new());

        var warehouseItem = warehouse!.Items.FirstOrDefault(wh
            => wh.ProductId == request.ProductId && wh.QuantityPerRoll == request.QuantityPerRoll);

        await context.BeginTransactionAsync(cancellationToken);

        if (warehouseItem is null)
            warehouse.Items.Add(mapper.Map<WarehouseItem>(request));
        else
        {
            warehouseItem.CountRoll += request.CountRoll;
            warehouseItem.TotalQuantity += request.TotalQuantity;
            warehouseItem.Price = request.Price;
        }

        var supply = mapper.Map<Supply>(request);
        context.Supplies.Add(mapper.Map<Supply>(request));

        await context.CommitTransactionAsync(cancellationToken);
        return supply.Id;
    }
}