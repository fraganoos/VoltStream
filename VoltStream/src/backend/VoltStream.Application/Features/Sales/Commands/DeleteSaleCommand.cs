namespace VoltStream.Application.Features.Sales.Commands;

using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using VoltStream.Application.Commons.Exceptions;
using VoltStream.Application.Commons.Interfaces;
using VoltStream.Domain.Entities;

public record DeleteSaleCommand(long Id) : IRequest<bool>;

public class DeleteSaleCommandHandler(
    IAppDbContext context)
    : IRequestHandler<DeleteSaleCommand, bool>
{
    public async Task<bool> Handle(DeleteSaleCommand request, CancellationToken cancellationToken)
    {
        var sale = await context.Sales
            .Include(s => s.CustomerOperation)
            .Include(s => s.Customer)
                .ThenInclude(c => c.Account)
            .Include(s => s.Discount)
            .Include(s => s.SaleItems)
            .FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Sale), nameof(request.Id), request.Id);

        var warehouse = await context.Warehouses
            .Include(wh => wh.Items)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException(nameof(Warehouse));


        foreach (var item in sale.SaleItems)
        {
            var residue = warehouse.Items.FirstOrDefault(r => r.ProductId == item.ProductId && r.QuantityPerRoll == item.QuantityPerRoll)
                ?? throw new NotFoundException(nameof(WarehouseItem), nameof(item.Id), item.Id);

            var countRoll = (int)item.TotalQuantity / item.QuantityPerRoll;
            var residueItem = item.TotalQuantity % item.QuantityPerRoll;

            residue.CountRoll += countRoll;
            residue.TotalQuantity += item.TotalQuantity - residueItem;

            if (residueItem > 0)
            {
                var existItem = await context.WarehouseItems.FirstOrDefaultAsync(i => i.ProductId == item.ProductId && i.QuantityPerRoll == residueItem, cancellationToken);

                if (existItem is null)
                    context.WarehouseItems.Add(new()
                    {
                        CountRoll = 1,
                        ProductId = item.ProductId,
                        QuantityPerRoll = residueItem,
                        DiscountPercent = item.DiscountPersent,
                        Price = item.Price,
                        TotalQuantity = residueItem,
                        Warehouse = warehouse
                    });
                else
                {
                    existItem.CountRoll += 1;
                    existItem.DiscountPercent = item.DiscountPersent;
                    existItem.Price = item.Price;
                    existItem.TotalQuantity += residueItem;
                }
            }
        }


        await context.BeginTransactionAsync(cancellationToken);

        sale.Customer.Account.CurrentSumm += sale.Summa;
        sale.Customer.Account.DiscountSumm -= sale.Discount;
        sale.IsDeleted = true;
        sale.CustomerOperation.IsDeleted = true;
        sale.DiscountOperation.IsDeleted = true;

        return await context.CommitTransactionAsync(cancellationToken);
    }
}
